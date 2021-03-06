using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum IndIEDest
    {
        [Description("Contribuinte ICMS")]
        [XmlEnum("1")]
        ContribuinteICMS = 1,
        [Description("Contribuinte isento de inscrição")]
        [XmlEnum("2")]
        Isento = 2,
        [Description("Não Contribuinte")]
        [XmlEnum("9")]
        NaoContribuinte = 9
    }
}
