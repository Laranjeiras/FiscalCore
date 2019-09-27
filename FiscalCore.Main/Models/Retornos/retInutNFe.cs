
using FiscalCore.Main.Models.Signatures;
using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Retornos
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class retInutNFe
    {
        [XmlAttribute]
        public string versao { get; set; }

        public infInutRet infInut { get; set; }

        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }
}
