using System;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Rtc;
using FiscalCore.NotaFiscal.Informacoes.Total;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.NotaFiscal.Rtc;

public sealed class IBSCBSSerializacaoTests
{
    [Fact]
    public void Imposto_SemIBSCBS_NaoSerializaGrupoRtc()
    {
        var xml = XmlUtils.ClasseParaXmlString(new imposto { PIS = PisOutros() });

        Assert.DoesNotContain("IBSCBS", xml);
        Assert.DoesNotContain("gIBSCBS", xml);
    }

    [Fact]
    public void Total_SemIBSCBSTot_NaoSerializaGrupoRtc()
    {
        var xml = XmlUtils.ClasseParaXmlString(new total { ICMSTot = new ICMSTot { vNF = 10m } });

        Assert.DoesNotContain("IBSCBSTot", xml);
    }

    [Fact]
    public void Imposto_ComIBSCBS_SerializaDepoisDosGruposExistentesComCasasDoLeiaute()
    {
        var imposto = new imposto
        {
            PIS = PisOutros(),
            IBSCBS = new IBSCBS
            {
                CST = "000",
                cClassTrib = "000001",
                gIBSCBS = new gIBSCBS
                {
                    vBC = 100m,
                    gIBSUF = new gIBSUF { pIBSUF = 0.1m, vIBSUF = 0.1m },
                    gIBSMun = new gIBSMun { pIBSMun = 0m, vIBSMun = 0m },
                    vIBS = 0.1m,
                    gCBS = new gCBS { pCBS = 0.9m, vCBS = 0.9m }
                }
            }
        };

        var xml = XmlUtils.ClasseParaXmlString(imposto);

        Assert.True(xml.IndexOf("<PIS>", StringComparison.Ordinal) < xml.IndexOf("<IBSCBS>", StringComparison.Ordinal));
        Assert.Contains("<CST>000</CST><cClassTrib>000001</cClassTrib>", xml);
        Assert.Contains("<gIBSUF><pIBSUF>0.1000</pIBSUF><vIBSUF>0.10</vIBSUF></gIBSUF>", xml);
        Assert.Contains("<gIBSMun><pIBSMun>0.0000</pIBSMun><vIBSMun>0.00</vIBSMun></gIBSMun>", xml);
        Assert.Contains("<vIBS>0.10</vIBS><gCBS><pCBS>0.9000</pCBS><vCBS>0.90</vCBS></gCBS>", xml);
        Assert.DoesNotContain("gRed", xml);
    }

    [Fact]
    public void GrupoComReducao_SerializaGRedEntreAliquotaEValor()
    {
        var gCbs = new gCBS
        {
            pCBS = 8.8m,
            gRed = new gRed { pRedAliq = 60m, pAliqEfet = 3.52m },
            vCBS = 3.52m
        };

        var xml = XmlUtils.ClasseParaXmlString(gCbs);

        Assert.Contains("<pCBS>8.8000</pCBS><gRed><pRedAliq>60.0000</pRedAliq><pAliqEfet>3.5200</pAliqEfet></gRed><vCBS>3.52</vCBS>", xml);
    }

    [Fact]
    public void Total_ComIBSCBSTot_SerializaDepoisDoICMSTotNaOrdemDoLeiaute()
    {
        var total = new total
        {
            ICMSTot = new ICMSTot { vNF = 100m },
            IBSCBSTot = new IBSCBSTot
            {
                vBCIBSCBS = 100m,
                gIBS = new gIBSTot
                {
                    gIBSUF = new gIBSUFTot { vIBSUF = 0.1m },
                    gIBSMun = new gIBSMunTot(),
                    vIBS = 0.1m
                },
                gCBS = new gCBSTot { vCBS = 0.9m }
            }
        };

        var xml = XmlUtils.ClasseParaXmlString(total);

        Assert.True(xml.IndexOf("</ICMSTot>", StringComparison.Ordinal) < xml.IndexOf("<IBSCBSTot>", StringComparison.Ordinal));
        Assert.Contains(
            "<IBSCBSTot><vBCIBSCBS>100.00</vBCIBSCBS><gIBS><gIBSUF><vDif>0.00</vDif><vDevTrib>0.00</vDevTrib><vIBSUF>0.10</vIBSUF></gIBSUF>"
            + "<gIBSMun><vDif>0.00</vDif><vDevTrib>0.00</vDevTrib><vIBSMun>0.00</vIBSMun></gIBSMun>"
            + "<vIBS>0.10</vIBS><vCredPres>0.00</vCredPres><vCredPresCondSus>0.00</vCredPresCondSus></gIBS>"
            + "<gCBS><vDif>0.00</vDif><vDevTrib>0.00</vDevTrib><vCBS>0.90</vCBS><vCredPres>0.00</vCredPres><vCredPresCondSus>0.00</vCredPresCondSus></gCBS></IBSCBSTot>",
            xml);
    }

    private static PIS PisOutros() =>
        new() { TipoPIS = new PISOutr { CST = CSTPIS.pis99, vBC = 0m, pPIS = 0m, vPIS = 0m } };
}
