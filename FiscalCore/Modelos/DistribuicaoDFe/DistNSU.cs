using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class DistNSU
    {
        [XmlElement("ultNSU")]
        public string UltNSU { get; set; }
    }
}
