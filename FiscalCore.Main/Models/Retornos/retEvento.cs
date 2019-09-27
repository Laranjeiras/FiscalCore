using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Retornos
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class retEvento
    {
        [XmlAttribute]
        public string versao { get; set; }

        public infEventoRet infEvento { get; set; }
    }
}
