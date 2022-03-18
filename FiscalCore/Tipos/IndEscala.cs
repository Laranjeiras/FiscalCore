using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eIndEscala
    {
        [XmlEnum("S")] S = 'S',
        [XmlEnum("N")] N = 'N'
    }
}
