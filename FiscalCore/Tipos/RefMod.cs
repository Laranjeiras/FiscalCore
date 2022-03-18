using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eRefMod
    {
        [XmlEnum("01")]
        Modelo = 1,

        [XmlEnum("02")]
        Modelo2 = 2 // Versão 4.00
    }
}
