using System.Xml.Serialization;

namespace FiscalCore.Modelos.Signatures
{
    public class CanonicalizationMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}