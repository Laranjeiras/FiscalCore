using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eModeloDocumento
    {
        [Description("NFe")]
        [XmlEnum("55")]
        NFe = 55,
        [Description("NFCe")]
        [XmlEnum("65")]
        NFCe = 65
    }
}
