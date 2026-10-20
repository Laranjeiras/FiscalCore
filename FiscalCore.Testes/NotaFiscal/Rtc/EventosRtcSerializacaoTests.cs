using System.Collections.Generic;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.NotaFiscal.Rtc;

public sealed class EventosRtcSerializacaoTests
{
    [Fact]
    public void AceiteDebito_SerializaCamposRtcSemImobilizacaoNemCancelamento()
    {
        var det = new detEvento
        {
            versao = "1.00",
            descEvento = "Aceite de débito na apuração por emissão de nota de crédito",
            cOrgaoAutor = eUF.RJ,
            tpAutor = "2",
            verAplic = "1.0",
            indAceitacao = "1"
        };

        var xml = XmlUtils.ClasseParaXmlString(det);

        Assert.Contains("<descEvento>Aceite de débito na apuração por emissão de nota de crédito</descEvento>", xml);
        Assert.Contains("<cOrgaoAutor>33</cOrgaoAutor>", xml);
        Assert.Contains("<tpAutor>2</tpAutor>", xml);
        Assert.Contains("<verAplic>1.0</verAplic>", xml);
        Assert.Contains("<indAceitacao>1</indAceitacao>", xml);
        Assert.DoesNotContain("gImobilizacao", xml);
        Assert.DoesNotContain("nProt", xml);
        Assert.DoesNotContain("xJust", xml);
        Assert.DoesNotContain("xCorrecao", xml);
    }

    [Fact]
    public void ImobilizacaoItem_SerializaGImobilizacaoSemAceitacaoNemCancelamento()
    {
        var det = new detEvento
        {
            versao = "1.00",
            descEvento = "Imobilização de Item",
            cOrgaoAutor = eUF.RJ,
            tpAutor = "2",
            verAplic = "1.0",
            gImobilizacao = new List<gImobilizacao>
            {
                new gImobilizacao
                {
                    nItem = 1,
                    vIBS = 120.50m,
                    vCBS = 55.20m,
                    gControleEstoque = new gControleEstoque
                    {
                        qImobilizado = 3.5000m,
                        uImobilizado = "UN"
                    }
                }
            }
        };

        var xml = XmlUtils.ClasseParaXmlString(det);

        Assert.Contains("<descEvento>Imobilização de Item</descEvento>", xml);
        Assert.Contains("<cOrgaoAutor>33</cOrgaoAutor>", xml);
        Assert.Contains("<tpAutor>2</tpAutor>", xml);
        Assert.Contains("<verAplic>1.0</verAplic>", xml);
        Assert.Contains("<gImobilizacao nItem=\"1\">", xml);
        Assert.Contains("<vIBS>120.50</vIBS>", xml);
        Assert.Contains("<vCBS>55.20</vCBS>", xml);
        Assert.Contains("<gControleEstoque>", xml);
        Assert.Contains("<qImobilizado>3.5000</qImobilizado>", xml);
        Assert.Contains("<uImobilizado>UN</uImobilizado>", xml);
        Assert.DoesNotContain("indAceitacao", xml);
        Assert.DoesNotContain("nProt", xml);
        Assert.DoesNotContain("xJust", xml);
        Assert.DoesNotContain("xCorrecao", xml);
    }

    [Fact]
    public void Cancelamento_NaoSerializaCamposRtc()
    {
        var det = new detEvento
        {
            versao = "1.00",
            descEvento = "Cancelamento",
            nProt = "133260000000001",
            xJust = "Cancelamento de teste pelo cliente"
        };

        var xml = XmlUtils.ClasseParaXmlString(det);

        Assert.Contains("<descEvento>Cancelamento</descEvento>", xml);
        Assert.Contains("<nProt>133260000000001</nProt>", xml);
        Assert.Contains("<xJust>Cancelamento de teste pelo cliente</xJust>", xml);
        Assert.DoesNotContain("cOrgaoAutor", xml);
        Assert.DoesNotContain("tpAutor", xml);
        Assert.DoesNotContain("verAplic", xml);
        Assert.DoesNotContain("indAceitacao", xml);
        Assert.DoesNotContain("gImobilizacao", xml);
    }
}
