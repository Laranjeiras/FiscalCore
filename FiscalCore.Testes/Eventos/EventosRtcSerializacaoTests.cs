using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.Eventos;

public class EventosRtcSerializacaoTests
{
    [Fact]
    public void SerializarEvento211128_AceiteDebito_GeraXmlCorretoEValidaContraXsd()
    {
        var infEvento = new infEventoEnv
        {
            Id = "ID2111283526101234567800019555001000000001100000001001",
            cOrgao = eUF.SP,
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
        Assert.Contains("<cOrgao>35</cOrgao>", xml);
        Assert.Contains("<descEvento>Aceite de débito na apuração por emissão de nota de crédito</descEvento>", xml);
        Assert.Contains("<cOrgaoAutor>35</cOrgaoAutor>", xml);
        Assert.Contains("<tpAutor>2</tpAutor>", xml);
        Assert.Contains("<verAplic>AlgoPlus 1.0</verAplic>", xml);
        Assert.Contains("<indAceitacao>1</indAceitacao>", xml);
        Assert.DoesNotContain("<nProt>", xml);
        Assert.DoesNotContain("<xCorrecao>", xml);
        Assert.DoesNotContain("<gImobilizacao>", xml);

        ValidarDetEventoContraSchema(xml, "e211128_v1.00.xsd");
    }

    [Fact]
    public void SerializarEvento211130_ImobilizacaoItem_GeraXmlCorretoEValidaContraXsd()
    {
        var infEvento = new infEventoEnv
        {
            Id = "ID2111303526101234567800019555001000000001100000001001",
            cOrgao = eUF.SP,
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
        Assert.Contains("<cOrgao>35</cOrgao>", xml);
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

        ValidarDetEventoContraSchema(xml, "e211130_v1.00.xsd");
    }

    private static void ValidarDetEventoContraSchema(string xmlCompleto, string schemaFileName)
    {
        var match = Regex.Match(xmlCompleto, @"<detEvento\b[^>]*>.*?</detEvento>", RegexOptions.Singleline);
        Assert.True(match.Success, "Elemento detEvento não encontrado no XML");

        var detEventoXml = match.Value;
        if (!detEventoXml.Contains("xmlns="))
        {
            detEventoXml = detEventoXml.Replace("<detEvento", "<detEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\"");
        }

        var schemaDir = Path.Combine(AppContext.BaseDirectory, "Schemas", "Eventos_RTC");
        var tiposBasicosPath = Path.Combine(schemaDir, "tiposBasico_v1.03.xsd");
        var schemaPath = Path.Combine(schemaDir, schemaFileName);
        Assert.True(File.Exists(schemaPath), $"Schema {schemaFileName} não encontrado em {schemaDir}");

        var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
        if (File.Exists(tiposBasicosPath))
        {
            schemas.Add("http://www.portalfiscal.inf.br/nfe", tiposBasicosPath);
        }
        schemas.Add("http://www.portalfiscal.inf.br/nfe", schemaPath);

        var erros = new List<string>();
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemas
        };
        settings.ValidationEventHandler += (sender, args) =>
        {
            if (args.Severity == XmlSeverityType.Error)
                erros.Add(args.Message);
        };

        using var stringReader = new StringReader(detEventoXml);
        using var reader = XmlReader.Create(stringReader, settings);
        while (reader.Read()) { }

        Assert.Empty(erros);
    }
}
