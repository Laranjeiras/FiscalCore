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
    public void EhEventoAdquirente_IdentificaEventosCorretamente(eTipoEventoNFe tipo, bool esperado)
    {
        var resultado = SefazEndpointResolver.EhEventoAdquirente(tipo);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData(eTipoEventoNFe.AceiteDebitoNFeCredito, eUF.SP, eUF.AN)]
    [InlineData(eTipoEventoNFe.ImobilizacaoItemNFe, eUF.RJ, eUF.AN)]
    [InlineData(eTipoEventoNFe.ConfirmacaoOperacao, eUF.MG, eUF.AN)]
    [InlineData(eTipoEventoNFe.NFeCancelamento, eUF.RJ, eUF.RJ)]
    [InlineData(eTipoEventoNFe.NFeCartaCorrecao, eUF.RJ, eUF.RJ)]
    public void ObterUfDestino_RoteiaParaAmbienteNacionalOuEstadual(eTipoEventoNFe tipo, eUF ufEmitente, eUF ufEsperada)
    {
        var ufDestino = SefazEndpointResolver.ObterUfDestino(tipo, ufEmitente);
        Assert.Equal(ufEsperada, ufDestino);
    }

    [Fact]
    public void ObterUrlEvento_RoteiaParaAmbienteNacionalEmHomologacaoEProducao()
    {
        var urlHomolog = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.AceiteDebitoNFeCredito,
            eTipoAmbiente.Homologacao,
            eModeloDocumento.NFe,
            eUF.SP);

        Assert.Equal(eUF.AN, urlHomolog.UF);
        Assert.Contains("hom1.nfe.fazenda.gov.br", urlHomolog.Url);

        var urlProd = SefazEndpointResolver.ObterUrlEvento(
            eTipoEventoNFe.ImobilizacaoItemNFe,
            eTipoAmbiente.Producao,
            eModeloDocumento.NFe,
            eUF.RJ);

        Assert.Equal(eUF.AN, urlProd.UF);
        Assert.Contains("www.nfe.fazenda.gov.br", urlProd.Url);
    }
}
