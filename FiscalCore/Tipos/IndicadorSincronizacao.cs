using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eIndicadorSincronizacao
    {
        [XmlEnum("0")]
        Assincrono = 0,
        [XmlEnum("1")]
        Sincrono = 1
    }
}
