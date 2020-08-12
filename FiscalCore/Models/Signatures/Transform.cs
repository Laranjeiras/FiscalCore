using System.Xml.Serialization;

namespace FiscalCore.Modelos.Signatures
{
    public class Transform
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}