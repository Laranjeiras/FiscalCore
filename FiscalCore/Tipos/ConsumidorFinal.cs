using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eConsumidorFinal
    {
        [XmlEnum("0")]
        Nao = 0,
        [XmlEnum("1")]
        Sim = 1
    }
}
