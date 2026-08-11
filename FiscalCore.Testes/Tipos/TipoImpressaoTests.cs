using FiscalCore.Tipos;
using System.Linq;
using System.Xml.Serialization;
using Xunit;

namespace FiscalCore.Testes.Tipos;

public sealed class TipoImpressaoTests
{
    [Fact]
    public void SimplificadoTipo2_DeveManterCodigoSeisNoContratoXml()
    {
        var membro = typeof(eTipoImpressao)
            .GetMember(nameof(eTipoImpressao.SimplificadoTipo2))
            .Single();
        var atributo = Assert.Single(
            membro.GetCustomAttributes(typeof(XmlEnumAttribute), inherit: false));

        Assert.Equal(6, (int)eTipoImpressao.SimplificadoTipo2);
        Assert.Equal("6", ((XmlEnumAttribute)atributo).Name);
    }
}
