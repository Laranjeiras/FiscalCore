using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Signatures
{
    public class CanonicalizationMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}