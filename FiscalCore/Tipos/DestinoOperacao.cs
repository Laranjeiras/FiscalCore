using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eDestinoOperacao
    {
        [XmlEnum("1")]
        Interna = 1,
        [XmlEnum("2")]
        Interestadual = 2,
        [XmlEnum("3")]
        Exterior = 3
    }
}
