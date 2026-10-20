using System;
using System.Collections.Generic;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.Eventos;

public class EventosRtcSerializacaoTests
{
    [Fact]
    public void SerializarEvento211128_AceiteDebito_GeraXmlCorreto()
    {
        var infEvento = new infEventoEnv
        {
            Id = "ID2111283526101234567800019555001000000001100000001001",
            cOrgao = eUF.AN,
            tpAmb = eTipoAmbiente.Homologacao,
            CNPJ = "12345678000195",
            chNFe = "35261012345678000195550010000000011000000010",
            dhEvento = new DateTime(2026, 10, 20, 10, 30, 0, DateTimeKind.Utc),
            tpEvento = eTipoEventoNFe.AceiteDebitoNFeCredito,
            nSeqEvento = 1,
            verEvento = "1.00",
            detEvento = new detEvento
            {
                versao = "1.00",
                descEvento = "Aceite de débito na apuração por emissão de nota de crédito",
                cOrgaoAutor = eUF.SP,
                tpAutor = "2",
                verAplic = "AlgoPlus 1.0",
                indAceitacao = "1"
            }
        };

        var ev = evento.CriarEvento("1.00", infEvento);
        var env = envEvento.Criar("1.00", 1, ev);

        var xml = XmlUtils.ClasseParaXmlString(env);

        Assert.Contains("<tpEvento>211128</tpEvento>", xml);
        Assert.Contains("<descEvento>Aceite de débito na apuração por emissão de nota de crédito</descEvento>", xml);
        Assert.Contains("<cOrgaoAutor>35</cOrgaoAutor>", xml);
        Assert.Contains("<tpAutor>2</tpAutor>", xml);
        Assert.Contains("<verAplic>AlgoPlus 1.0</verAplic>", xml);
        Assert.Contains("<indAceitacao>1</indAceitacao>", xml);
        Assert.DoesNotContain("<nProt>", xml);
        Assert.DoesNotContain("<xCorrecao>", xml);
        Assert.DoesNotContain("<gImobilizacao>", xml);
    }

    [Fact]
    public void SerializarEvento211130_ImobilizacaoItem_GeraXmlCorreto()
    {
        var infEvento = new infEventoEnv
        {
            Id = "ID2111303526101234567800019555001000000001100000001001",
            cOrgao = eUF.AN,
            tpAmb = eTipoAmbiente.Homologacao,
            CNPJ = "12345678000195",
            chNFe = "35261012345678000195550010000000011000000010",
            dhEvento = new DateTime(2026, 10, 20, 10, 30, 0, DateTimeKind.Utc),
            tpEvento = eTipoEventoNFe.ImobilizacaoItemNFe,
            nSeqEvento = 1,
            verEvento = "1.00",
            detEvento = new detEvento
            {
                versao = "1.00",
                descEvento = "Imobilização de Item",
                cOrgaoAutor = eUF.SP,
                tpAutor = "2",
                verAplic = "AlgoPlus 1.0",
                gImobilizacao = new List<gImobilizacao>
                {
                    new gImobilizacao
                    {
                        nItem = 1,
                        vIBS = 15.50m,
                        vCBS = 30.25m,
                        gControleEstoque = new gControleEstoque
                        {
                            qImobilizado = 2.0000m,
                            uImobilizado = "UN"
                        }
                    }
                }
            }
        };

        var ev = evento.CriarEvento("1.00", infEvento);
        var env = envEvento.Criar("1.00", 1, ev);

        var xml = XmlUtils.ClasseParaXmlString(env);

        Assert.Contains("<tpEvento>211130</tpEvento>", xml);
        Assert.Contains("<descEvento>Imobilização de Item</descEvento>", xml);
        Assert.Contains("<cOrgaoAutor>35</cOrgaoAutor>", xml);
        Assert.Contains("<tpAutor>2</tpAutor>", xml);
        Assert.Contains("<verAplic>AlgoPlus 1.0</verAplic>", xml);
        Assert.Contains("<gImobilizacao nItem=\"1\">", xml);
        Assert.Contains("<vIBS>15.50</vIBS>", xml);
        Assert.Contains("<vCBS>30.25</vCBS>", xml);
        Assert.Contains("<gControleEstoque>", xml);
        Assert.Contains("<qImobilizado>2.0000</qImobilizado>", xml);
        Assert.Contains("<uImobilizado>UN</uImobilizado>", xml);
        Assert.DoesNotContain("<indAceitacao>", xml);
        Assert.DoesNotContain("<nProt>", xml);
    }
}
