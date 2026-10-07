using System;
using System.Security.Cryptography;
using System.Text;
using FiscalCore.Configuracoes;
using FiscalCore.Danfe.NFCe;
using FiscalCore.NotaFiscal;
using FiscalCore.NotaFiscal.Informacoes;
using FiscalCore.NotaFiscal.Informacoes.Identificacao;
using FiscalCore.Tipos;
using Xunit;

namespace FiscalCore.Testes.Danfe.NFCe;

public sealed class NfceQrCodeTests
{
    private const string Chave = "33260929310114000110650010000017621880103811";
    private const string UrlConsulta = "https://www4.fazenda.rj.gov.br/consultaNFCe/QRCode?p=";

    // Fictício, no mesmo formato GUID dos CSCs emitidos pela SVRS.
    private const string Csc = "0A1B2C3D-4E5F-4A6B-8C7D-9E0F1A2B3C4D";

    [Fact]
    public void ObterUrlQrCode_CscNoFormatoGuid_MontaUrlComIdTokenEHashDoCsc()
    {
        var nfe = new NFe
        {
            infNFe = new infNFe
            {
                Id = "NFe" + Chave,
                ide = new ide { tpAmb = eTipoAmbiente.Homologacao, tpEmis = eTipoEmissao.Normal },
            },
        };
        var configDanfe = new ConfiguracaoDanfe { NFCeUrlConsultaQrCodeSefaz = UrlConsulta };

        var url = NfceQrCode.ObterUrlQrCode(nfe, configDanfe, "000001", Csc);

        const string dadosBase = Chave + "|2|2|1";
        var hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(dadosBase + Csc))).ToLowerInvariant();
        Assert.Equal($"{UrlConsulta}{dadosBase}|{hash}", url);
    }
}
