using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    public class retEvento
    {
        [XmlElement("infEvento")]
        public retInfEvento InfEvento { get; set; }

        [XmlAttribute("versao")]
        public decimal Versao { get; set; }
    }
}
