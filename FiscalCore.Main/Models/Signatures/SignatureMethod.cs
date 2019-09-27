using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Signatures
{
    public class SignatureMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}