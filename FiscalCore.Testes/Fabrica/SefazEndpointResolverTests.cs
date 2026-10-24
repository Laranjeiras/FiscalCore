using FiscalCore.Fabrica;
using FiscalCore.Tipos;
using Xunit;

namespace FiscalCore.Testes.Fabrica;

public class SefazEndpointResolverTests
{
    [Theory]
    [InlineData(eTipoEventoNFe.AceiteDebitoNFeCredito, true)]
    [InlineData(eTipoEventoNFe.ImobilizacaoItemNFe, true)]
    [InlineData(eTipoEventoNFe.NFeCancelamento, false)]
    [InlineData(eTipoEventoNFe.NFeCartaCorrecao, false)]
    [InlineData(eTipoEventoNFe.ConfirmacaoOperacao, false)]
    public void EhEventoAdquirente_IdentificaEventosCorretamente(eTipoEventoNFe tipo, bool esperado)
    {
        var resultado = SefazEndpointResolver.EhEventoAdquirente(tipo);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData(eTipoEventoNFe.AceiteDebitoNFeCredito, eUF.SP, eUF.SP)]
    [InlineData(eTipoEventoNFe.ImobilizacaoItemNFe, eUF.RJ, eUF.RJ)]
    [InlineData(eTipoEventoNFe.ConfirmacaoOperacao, eUF.MG, eUF.AN)]
    [InlineData(eTipoEventoNFe.CienciaOperacao, eUF.BA, eUF.AN)]
    [InlineData(eTipoEventoNFe.NFeCancelamento, eUF.RJ, eUF.RJ)]
    [InlineData(eTipoEventoNFe.NFeCartaCorrecao, eUF.RS, eUF.RS)]
    public void ObterUfDestino_RoteiaParaAmbienteNacionalOuEstadual(eTipoEventoNFe tipo, eUF ufEmitente, eUF ufEsperada)
    {
        var ufDestino = SefazEndpointResolver.ObterUfDestino(tipo, ufEmitente);
        Assert.Equal(ufEsperada, ufDestino);
    }

    [Fact]
    public void ObterUrlEvento_RoteiaEventosRtcParaSVRS_ConformeNT2025002()
    {
        var urlHomolog = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.AceiteDebitoNFeCredito,
            eTipoAmbiente.Homologacao,
            eModeloDocumento.NFe,
            eUF.SP);

        Assert.Contains("nfe-homologacao.svrs.rs.gov.br", urlHomolog.Url);
        Assert.Contains("recepcaoevento4.asmx", urlHomolog.Url);

        var urlProd = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.ImobilizacaoItemNFe,
            eTipoAmbiente.Producao,
            eModeloDocumento.NFe,
            eUF.RJ);

        Assert.Contains("nfe.svrs.rs.gov.br", urlProd.Url);
        Assert.Contains("recepcaoevento4.asmx", urlProd.Url);
    }

    [Fact]
    public void ObterUrlEvento_RoteiaManifestacaoTradicionalParaAmbienteNacional()
    {
        var urlHomolog = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.ConfirmacaoOperacao,
            eTipoAmbiente.Homologacao,
            eModeloDocumento.NFe,
            eUF.SP);

        Assert.Equal(eUF.AN, urlHomolog.UF);
        Assert.Contains("hom1.nfe.fazenda.gov.br", urlHomolog.Url);

        var urlProd = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.ConfirmacaoOperacao,
            eTipoAmbiente.Producao,
            eModeloDocumento.NFe,
            eUF.RJ);

        Assert.Equal(eUF.AN, urlProd.UF);
        Assert.Contains("www.nfe.fazenda.gov.br", urlProd.Url);
    }
}
