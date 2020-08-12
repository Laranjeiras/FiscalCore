using System.Xml.Serialization;

namespace FiscalCore.Modelos.Signatures
{
    public class DigestMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}