using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Signatures
{
    public class Transform
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}