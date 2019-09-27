using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Eventos
{
    public class detEvento
    {
        [XmlAttribute]
        public string versao { get; set; }

        public string descEvento { get; set; }

        public string nProt { get; set; }

        public string xJust { get; set; }
    }
}
