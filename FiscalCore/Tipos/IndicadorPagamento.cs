using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eIndicadorPagamento
    {
        [XmlEnum("0")]
        [Description("A Vista")]
        AVista,
        [XmlEnum("1")]
        [Description("Prazo")]
        Prazo
    }
}
