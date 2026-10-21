using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eFinalidade
    {
        [XmlEnum("1")]
        Normal = 1,
        [XmlEnum("2")]
        Complementar = 2,
        [XmlEnum("3")]
        Ajuste = 3,
        [XmlEnum("4")]
        Devolucao = 4,
        [XmlEnum("5")]
        NFeCredito = 5,
        [XmlEnum("6")]
        NFeDebito = 6
    }
}
