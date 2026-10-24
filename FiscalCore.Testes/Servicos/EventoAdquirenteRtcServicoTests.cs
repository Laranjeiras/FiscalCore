using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using AlgoPlus.Storage;
using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Configuracoes.Emitente;
using FiscalCore.Exceptions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Servicos;
using FiscalCore.Servicos.NotaFiscal.Eventos;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FiscalCore.Testes.Servicos;

public sealed class EventoAdquirenteRtcServicoTests
{
    private const string CnpjEmitente = "11222333000181";
    private const string Chave = "33260811222333000181550010000000011000000016";
    private const string Senha = "senha-de-teste";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("2")]
    [InlineData("9")]
    public async Task TransmitirEventoAsync_AceiteDebitoSemIndAceitacaoValido_DeveLancarFalhaValidacaoException(string? indAceitacao)
    {
        var cfg = CriarConfiguracao();
        var transmissor = new TransmissorCapturador();
        var servico = new EventoAdquirenteRtcServico(cfg, new StorageFake(), transmissor, NullLogger<EventoAdquirenteRtcServico>.Instance);

        var info = new InfoEventoAdquirenteRtc
        {
            ChaveAcesso = Chave,
            nSeqEvento = 1,
            TipoEvento = eTipoEventoNFe.AceiteDebitoNFeCredito,
            IndAceitacao = indAceitacao!
        };

        await Assert.ThrowsAsync<FalhaValidacaoException>(() => servico.TransmitirEventoAsync(info));
    }

    [Fact]
    public async Task TransmitirEventoAsync_ImobilizacaoSemItens_DeveLancarFalhaValidacaoException()
    {
        var cfg = CriarConfiguracao();
        var transmissor = new TransmissorCapturador();
        var servico = new EventoAdquirenteRtcServico(cfg, new StorageFake(), transmissor, NullLogger<EventoAdquirenteRtcServico>.Instance);

        var info = new InfoEventoAdquirenteRtc
        {
            ChaveAcesso = Chave,
            nSeqEvento = 1,
            TipoEvento = eTipoEventoNFe.ImobilizacaoItemNFe,
            ItensImobilizados = new List<gImobilizacao>()
        };

        await Assert.ThrowsAsync<FalhaValidacaoException>(() => servico.TransmitirEventoAsync(info));
    }

    [Fact]
    public async Task TransmitirEventoAsync_AceiteDebitoValido_DeveRotearParaSVRSERepassarCancellationToken()
    {
        var cfg = CriarConfiguracao(eTipoAmbiente.Homologacao);
        cfg.VersaoProc = "Pyre 3.3.0";
        var transmissor = new TransmissorCapturador();
        var servico = new EventoAdquirenteRtcServico(cfg, new StorageFake(), transmissor, NullLogger<EventoAdquirenteRtcServico>.Instance);

        var info = new InfoEventoAdquirenteRtc
        {
            ChaveAcesso = Chave,
            nSeqEvento = 1,
            TipoEvento = eTipoEventoNFe.AceiteDebitoNFeCredito,
            IndAceitacao = "1"
        };

        using var cts = new CancellationTokenSource();
        var resultado = await servico.TransmitirEventoAsync(info, cts.Token);

        Assert.NotNull(transmissor.UrlCapturada);
        Assert.Contains("nfe-homologacao.svrs.rs.gov.br", transmissor.UrlCapturada.Url);
        Assert.Contains("recepcaoevento4.asmx", transmissor.UrlCapturada.Url);
        Assert.Equal(cts.Token, transmissor.CancellationTokenCapturado);

        Assert.NotNull(transmissor.EnvelopeCapturado);
        var envelopeXml = transmissor.EnvelopeCapturado.OuterXml;
        Assert.Contains("<cOrgao>33</cOrgao>", envelopeXml);
        Assert.Contains("<cOrgaoAutor>33</cOrgaoAutor>", envelopeXml);
        Assert.Contains("<verAplic>Pyre 3.3.0</verAplic>", envelopeXml);
        Assert.Contains("<indAceitacao>1</indAceitacao>", envelopeXml);
    }

    private static ConfiguracaoServico CriarConfiguracao(eTipoAmbiente ambiente = eTipoAmbiente.Homologacao)
    {
        var cert = Certificado(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(10));
        var cfgCert = new ConfiguracaoCertificado(cert);
        var emit = new emit
        {
            CNPJ = CnpjEmitente,
            xNome = "EMPRESA TESTE",
            enderEmit = new enderEmit
            {
                cMun = 3304557,
                xLgr = "Rua Teste",
                nro = "100",
                xBairro = "Centro",
                UF = eUF.RJ
            }
        };

        return new ConfiguracaoServico(
            ambiente,
            eUF.RJ,
            cfgCert,
            emit,
            new ConfiguracaoCsc("000001", "0123456789"),
            null!
        )
        {
            DiretorioSchemas = AppContext.BaseDirectory
        };
    }

    private static X509Certificate2 Certificado(DateTimeOffset inicio, DateTimeOffset fim)
    {
        using var rsa = RSA.Create(2048);
        var requisicao = new CertificateRequest("CN=Teste NFe", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var gerado = requisicao.CreateSelfSigned(inicio, fim);
        return new X509Certificate2(gerado.Export(X509ContentType.Pfx, Senha), Senha, X509KeyStorageFlags.Exportable);
    }

    private sealed class TransmissorCapturador : ITransmitirSefazCommand
    {
        public UrlSefaz? UrlCapturada { get; private set; }
        public XmlDocument? EnvelopeCapturado { get; private set; }
        public CancellationToken CancellationTokenCapturado { get; private set; }

        public Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope)
        {
            UrlCapturada = url;
            EnvelopeCapturado = envelope;
            return Task.FromResult(XmlRetornoSucesso());
        }

        public Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope, CancellationToken cancellation)
        {
            UrlCapturada = url;
            EnvelopeCapturado = envelope;
            CancellationTokenCapturado = cancellation;
            return Task.FromResult(XmlRetornoSucesso());
        }

        private static string XmlRetornoSucesso() =>
            @"<soapenv:Envelope xmlns:soapenv=""http://www.w3.org/2003/05/soap-envelope"">
                <soapenv:Body>
                    <nfeResultMsg xmlns=""http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4"">
                        <retEnvEvento versao=""1.00"" xmlns=""http://www.portalfiscal.inf.br/nfe"">
                            <idLote>1</idLote>
                            <tpAmb>2</tpAmb>
                            <verAplic>SVRS</verAplic>
                            <cOrgao>33</cOrgao>
                            <cStat>128</cStat>
                            <xMotivo>Lote de Evento Processado</xMotivo>
                            <retEvento versao=""1.00"">
                                <infEvento>
                                    <tpAmb>2</tpAmb>
                                    <verAplic>SVRS</verAplic>
                                    <cOrgao>33</cOrgao>
                                    <cStat>135</cStat>
                                    <xMotivo>Evento registrado e vinculado a NF-e</xMotivo>
                                    <chNFe>" + Chave + @"</chNFe>
                                    <tpEvento>211128</tpEvento>
                                    <nSeqEvento>1</nSeqEvento>
                                    <dhRegEvento>2026-10-24T00:00:00-03:00</dhRegEvento>
                                    <nProt>133260000000001</nProt>
                                </infEvento>
                            </retEvento>
                        </retEnvEvento>
                    </nfeResultMsg>
                </soapenv:Body>
            </soapenv:Envelope>";
    }

    private sealed class StorageFake : IStorageContext
    {
        public IStorage GetStorage(string name) => null!;
        public Task<ReturnFileInfo> SaveAsync(string filename, string content, string storageName, CancellationToken cancellation)
            => Task.FromResult(new ReturnFileInfo());
        public Task<ReturnFileInfo> SaveAsync(string filename, byte[] content, string storageName, CancellationToken cancellation)
            => Task.FromResult(new ReturnFileInfo());
    }
}
