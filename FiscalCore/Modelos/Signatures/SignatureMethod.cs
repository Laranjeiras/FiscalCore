#nullable disable
#pragma warning disable CS8981
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Signatures
{
    public class SignatureMethod
    {
        [XmlAttribute]
        public string Algorithm { get; set; }
    }
}