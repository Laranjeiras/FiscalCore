using FiscalCore.NotaFiscal.Informacoes.Emitente;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System.Linq;
using System.Xml.Serialization;
using Xunit;

namespace FiscalCore.Testes.Tipos;

public sealed class CRTTests
{
    [Theory]
    [InlineData(eCRT.SimplesNacional, 1, "1")]
    [InlineData(eCRT.SimplesNacionalExcessoSublimite, 2, "2")]
    [InlineData(eCRT.RegimeNormal, 3, "3")]
    [InlineData(eCRT.SimplesNacionalMicroempreendedorIndividual, 4, "4")]
    public void Crt_DeveManterCodigoNumericoEXml(
        eCRT crt,
        int codigoEsperado,
        string codigoXmlEsperado)
    {
        var membro = typeof(eCRT).GetMember(crt.ToString()).Single();
        var xmlEnum = Assert.Single(membro.GetCustomAttributes(typeof(XmlEnumAttribute), false));

        Assert.Equal(codigoEsperado, (int)crt);
        Assert.Equal(codigoXmlEsperado, ((XmlEnumAttribute)xmlEnum).Name);
    }

    [Fact]
    public void ClasseParaXmlString_CrtMei_DeveSerializarCodigoQuatro()
    {
        var emitente = new emit
        {
            CNPJ = "29310114000110",
            xNome = "Emitente MEI",
            IE = "123456789",
            CRT = eCRT.SimplesNacionalMicroempreendedorIndividual
        };

        var xml = XmlUtils.ClasseParaXmlString(emitente);

        Assert.Contains("<CRT>4</CRT>", xml);
    }
}
