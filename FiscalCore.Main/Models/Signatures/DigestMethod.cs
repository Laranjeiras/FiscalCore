using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Signatures
{
    public class DigestMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}