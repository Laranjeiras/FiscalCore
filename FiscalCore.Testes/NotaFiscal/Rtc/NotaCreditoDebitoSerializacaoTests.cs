using FiscalCore.NotaFiscal.Informacoes.Identificacao;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.NotaFiscal.Rtc;

public sealed class NotaCreditoDebitoSerializacaoTests
{
    private static ide CriarIdeBase()
    {
        return new ide
        {
            cUF = eUF.RJ,
            cNF = "00000001",
            natOp = "VENDA",
            mod = eModeloDocumento.NFe,
            serie = 1,
            nNF = 123,
            dhEmi = System.DateTimeOffset.UtcNow,
            tpNF = eTipoNF.Saida,
            idDest = eDestinoOperacao.Interna,
            cMunFG = 3304557,
            tpImp = eTipoImpressao.NormalRetrato,
            tpEmis = eTipoEmissao.Normal,
            tpAmb = eTipoAmbiente.Homologacao,
            finNFe = eFinalidade.Normal,
            procEmi = eProcessoEmissao.AplicativoContribuinte,
            verProc = "1.0"
        };
    }

    [Fact]
    public void Ide_NormalSemCreditoDebito_NaoSerializaTpNFCreditoNemTpNFDebitoNemNB01()
    {
        var ide = CriarIdeBase();

        var xml = XmlUtils.ClasseParaXmlString(ide);

        Assert.Contains("<finNFe>1</finNFe>", xml);
        Assert.DoesNotContain("tpNFCredito", xml);
        Assert.DoesNotContain("tpNFDebito", xml);
        Assert.DoesNotContain("NB01", xml);
    }

    [Fact]
    public void Ide_NotaCredito_SerializaFinNFe5ETpNFCredito()
    {
        var ide = CriarIdeBase();
        ide.finNFe = eFinalidade.NFeCredito;
        ide.tpNFCredito = eTipoNotaCredito.MultaEJuros;

        var xml = XmlUtils.ClasseParaXmlString(ide);

        Assert.Contains("<finNFe>5</finNFe>", xml);
        Assert.Contains("<tpNFCredito>01</tpNFCredito>", xml);
        Assert.DoesNotContain("tpNFDebito", xml);
    }

    [Fact]
    public void Ide_NotaDebito_SerializaFinNFe6ETpNFDebito()
    {
        var ide = CriarIdeBase();
        ide.finNFe = eFinalidade.NFeDebito;
        ide.tpNFDebito = eTipoNotaDebito.PagamentoAntecipado;

        var xml = XmlUtils.ClasseParaXmlString(ide);

        Assert.Contains("<finNFe>6</finNFe>", xml);
        Assert.Contains("<tpNFDebito>06</tpNFDebito>", xml);
        Assert.DoesNotContain("tpNFCredito", xml);
    }

    [Fact]
    public void NB01_ComVICMSPrevisto_SerializaQuandoInformado()
    {
        var nb01 = new NB01
        {
            vICMSPrevisto = 150.25m
        };

        var xml = XmlUtils.ClasseParaXmlString(nb01);

        Assert.Contains("<vICMSPrevisto>150.25</vICMSPrevisto>", xml);
    }

    [Fact]
    public void NB01_SemVICMSPrevisto_NaoSerializaTagVICMSPrevisto()
    {
        var nb01 = new NB01
        {
            vICMSPrevisto = null
        };

        var xml = XmlUtils.ClasseParaXmlString(nb01);

        Assert.DoesNotContain("vICMSPrevisto", xml);
    }

    [Fact]
    public void TipoEventoNFe_PreservaValoresLegadosEAdicionaEventosRtc()
    {
        Assert.Equal(110110, (int)eTipoEventoNFe.NFeCartaCorrecao);
        Assert.Equal(110111, (int)eTipoEventoNFe.NFeCancelamento);
        Assert.Equal(210200, (int)eTipoEventoNFe.ConfirmacaoOperacao);
        Assert.Equal(211128, (int)eTipoEventoNFe.AceiteDebitoNFeCredito);
        Assert.Equal(211130, (int)eTipoEventoNFe.ImobilizacaoItemNFe);
    }
}
