using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eVersaoServico
    {
        [XmlEnum("1.00")]
        [Description("1.00")]
        Versao100 = 100,
        [Description("2.00")]
        [XmlEnum("2.00")]
        Versao200 = 200,
        [Description("3.10")]
        [XmlEnum("3.10")]
        Versao310 = 310,
        [Description("4.00")]
        [XmlEnum("4.00")]
        Versao400 = 400
    }
}
