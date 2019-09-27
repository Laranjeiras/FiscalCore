using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Protocolos
{
    public class protNFe
    {
        public protNFe()
        {
            infProt = new infProt();
        }

        [XmlAttribute]
        public string versao { get; set; }

        public infProt infProt { get; set; }
    }
}