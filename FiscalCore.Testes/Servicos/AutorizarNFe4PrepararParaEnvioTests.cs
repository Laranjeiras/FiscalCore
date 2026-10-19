using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using AlgoPlus.Storage;
using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.NotaFiscal;
using FiscalCore.NotaFiscal.Informacoes;
using FiscalCore.NotaFiscal.Informacoes.Destinatario;
using FiscalCore.NotaFiscal.Informacoes.Detalhe;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal;
using FiscalCore.NotaFiscal.Informacoes.Emitente;
using FiscalCore.NotaFiscal.Informacoes.Identificacao;
using FiscalCore.NotaFiscal.Informacoes.Transporte;
using FiscalCore.Servicos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FiscalCore.Testes.Servicos;

/// <summary>
/// <see cref="AutorizarNFe4.PrepararParaEnvio(NFe)"/> expõe o preparo que o <c>Autorizar</c> faz antes de
/// transmitir (certificado, políticas, validações e assinatura) para quem grava o XML assinado antes do envio.
/// </summary>
public sealed class AutorizarNFe4PrepararParaEnvioTests
{
    private const string CnpjEmitente = "11222333000181";
    private const string Chave = "33260811222333000181650010000000011000000016";
    private const string Senha = "senha-de-teste";

    [Fact]
    public void PrepararParaEnvio_NFCeEmHomologacao_AplicaAsPoliticasEAssina()
    {
        var nfe = NovaNFe(eModeloDocumento.NFCe);

        var assinada = Autorizador(CertificadoValido()).PrepararParaEnvio(nfe);

        var info = assinada.infNFe;
        Assert.Equal(eConsumidorFinal.Sim, info.ide.indFinal);
        Assert.Null(info.det[0].imposto.IPI);
        Assert.Null(info.ide.dhSaiEnt);
        Assert.Equal(ePresencaComprador.Presencial, info.ide.indPres);
        Assert.Equal(eModalidadeFrete.SemFrete, info.transp.modFrete);
        Assert.Null(info.transp.veicTransp);
        Assert.Equal("NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL", info.dest.xNome);
        Assert.Equal("NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL", info.det[0].prod.xProd);
        Assert.NotNull(assinada.Signature);
    }

    [Fact]
    public void PrepararParaEnvio_NFe_MantemIPIEFrete()
    {
        var nfe = NovaNFe(eModeloDocumento.NFe);

        var assinada = Autorizador(CertificadoValido()).PrepararParaEnvio(nfe);

        Assert.NotNull(assinada.infNFe.det[0].imposto.IPI);
        Assert.NotNull(assinada.infNFe.transp.veicTransp);
        Assert.NotNull(assinada.Signature);
    }

    [Fact]
    public void PrepararParaEnvio_MesmoXmlDoPreparoManualDasPoliticas()
    {
        var certificado = CertificadoValido();
        var autorizador = Autorizador(certificado);
        var manual = new List<NFe> { NovaNFe(eModeloDocumento.NFCe) };

        TratarNFeAutorizacao.AplicarPoliticas(manual);
        ValidarNFeAutorizacao.ValidarPoliticas(manual, Configuracao(certificado));
        var esperado = XmlUtils.ClasseParaXmlString(autorizador.ValidarEAssinar(manual[0]));

        var obtido = XmlUtils.ClasseParaXmlString(autorizador.PrepararParaEnvio(NovaNFe(eModeloDocumento.NFCe)));

        Assert.Equal(esperado, obtido);
    }

    [Fact]
    public void PrepararParaEnvio_Lista_DevolveAsNotasAssinadasNaOrdem()
    {
        var primeira = NovaNFe(eModeloDocumento.NFe);
        var segunda = NovaNFe(eModeloDocumento.NFe);
        segunda.infNFe.Id = "NFe" + Chave.Substring(0, 43) + "9";

        var assinadas = Autorizador(CertificadoValido()).PrepararParaEnvio(new List<NFe> { primeira, segunda });

        Assert.Equal(2, assinadas.Count);
        Assert.Equal(primeira.infNFe.Id, assinadas[0].infNFe.Id);
        Assert.Equal(segunda.infNFe.Id, assinadas[1].infNFe.Id);
    }

    [Fact]
    public void PrepararParaEnvio_EmitenteDiferenteDoConfigurado_LancaSemAssinar()
    {
        var nfe = NovaNFe(eModeloDocumento.NFe);
        nfe.infNFe.emit.CNPJ = "11444777000161";

        var excecao = Assert.Throws<FalhaValidacaoException>(() => Autorizador(CertificadoValido()).PrepararParaEnvio(nfe));

        Assert.Contains("Emitente", excecao.Message);
        Assert.Null(nfe.Signature);
    }

    [Fact]
    public void PrepararParaEnvio_ModelosMisturadosNoLote_Lanca()
    {
        var lote = new List<NFe> { NovaNFe(eModeloDocumento.NFe), NovaNFe(eModeloDocumento.NFCe) };

        var excecao = Assert.Throws<FalhaValidacaoException>(() => Autorizador(CertificadoValido()).PrepararParaEnvio(lote));

        Assert.Contains("modelo", excecao.Message);
    }

    [Fact]
    public void PrepararParaEnvio_CertificadoExpirado_LancaAntesDasPoliticas()
    {
        var nfe = NovaNFe(eModeloDocumento.NFCe);
        var expirado = Certificado(DateTimeOffset.UtcNow.AddYears(-2), DateTimeOffset.UtcNow.AddDays(-1));

        Assert.Throws<ConfiguracaoException>(() => Autorizador(expirado).PrepararParaEnvio(nfe));

        Assert.NotNull(nfe.infNFe.det[0].imposto.IPI);
        Assert.Null(nfe.Signature);
    }

    // ----------------------------------------------------------------- infraestrutura de teste

    private static AutorizarNFe4 Autorizador(X509Certificate2 certificado)
        => new(Configuracao(certificado), new TransmissorNaoUsado(), new StorageNaoUsado(), NullLogger<AutorizarNFe4>.Instance);

    private static ConfiguracaoServico Configuracao(X509Certificate2 certificado)
        => new()
        {
            ConfigCertificado = new ConfiguracaoCertificado(certificado),
            CNPJEmitente = CnpjEmitente,
            TipoAmbiente = eTipoAmbiente.Homologacao,
            VersaoAutorizacaoNFe = eVersaoServico.Versao400
        };

    private static X509Certificate2 CertificadoValido()
        => Certificado(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));

    private static X509Certificate2 Certificado(DateTimeOffset inicio, DateTimeOffset fim)
    {
        using var rsa = RSA.Create(2048);
        var requisicao = new CertificateRequest("CN=Teste NFe", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var gerado = requisicao.CreateSelfSigned(inicio, fim);
        return new X509Certificate2(gerado.Export(X509ContentType.Pfx, Senha), Senha, X509KeyStorageFlags.Exportable);
    }

    private static NFe NovaNFe(eModeloDocumento modelo)
        => new()
        {
            infNFe = new infNFe
            {
                versao = "4.00",
                Id = "NFe" + Chave,
                ide = new ide
                {
                    cUF = eUF.RJ,
                    mod = modelo,
                    tpNF = eTipoNF.Saida,
                    tpImp = eTipoImpressao.NormalRetrato,
                    tpEmis = eTipoEmissao.Normal,
                    tpAmb = eTipoAmbiente.Homologacao,
                    finNFe = eFinalidade.Normal,
                    indFinal = eConsumidorFinal.Nao,
                    indPres = ePresencaComprador.Internet,
                    dhSaiEnt = new DateTimeOffset(2026, 8, 1, 10, 0, 0, TimeSpan.FromHours(-3))
                },
                emit = new emit { CNPJ = CnpjEmitente, xNome = "Emitente Teste", CRT = eCRT.SimplesNacional },
                dest = new dest { xNome = "Destinatario", indIEDest = indIEDest.NaoContribuinte },
                det = new List<det>
                {
                    new() { prod = new prod { xProd = "Produto" }, imposto = new imposto { IPI = new IPI() } }
                },
                transp = new transp { modFrete = eModalidadeFrete.PorContaEmitente, veicTransp = new veicTransp() }
            }
        };

    private sealed class TransmissorNaoUsado : ITransmitirSefazCommand
    {
        public Task<string> TransmitirAsync(UrlSefaz url, XmlDocument envelope)
            => throw new InvalidOperationException("O preparo não transmite.");
    }

    private sealed class StorageNaoUsado : IStorageContext
    {
        public IStorage GetStorage(string name) => null!;

        public Task<ReturnFileInfo> SaveAsync(string filename, string content, string storageName, System.Threading.CancellationToken cancellation)
            => throw new InvalidOperationException("O preparo não grava arquivos.");

        public Task<ReturnFileInfo> SaveAsync(string filename, byte[] content, string storageName, System.Threading.CancellationToken cancellation)
            => throw new InvalidOperationException("O preparo não grava arquivos.");
    }
}
