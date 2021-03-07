using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.DistribuicaoDFe.Modelos
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class consChNFe
    {
        [XmlElement("chNFe")]
        public string ChNFe { get; set; }
    }
}
