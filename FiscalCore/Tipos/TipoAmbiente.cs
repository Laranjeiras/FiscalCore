using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoAmbiente
    {
        [XmlEnum("1")]
        [Description("Produção")]
        Producao = 1,

        [XmlEnum("2")]
        [Description("Homologação")]
        Homologacao = 2
    }
}
