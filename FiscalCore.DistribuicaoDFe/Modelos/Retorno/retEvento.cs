using System.Xml.Serialization;

namespace FiscalCore.DistribuicaoDFe.Modelos.Retorno
{
    public class retEvento
    {
        [XmlElement("infEvento")]
        public retInfEvento InfEvento { get; set; }

        [XmlAttribute("versao")]
        public decimal Versao { get; set; }
    }
}
