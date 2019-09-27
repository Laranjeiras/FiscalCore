using FiscalCore.Main.Enums;
using FiscalCore.Main.Extensions;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Eventos
{
    public class infEventoEnv
    {
        [XmlAttribute]
        public string Id { get; set; }

        public eUF cOrgao { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public string CNPJ { get; set; }
        public string CPF { get; set; }
        
        public string chNFe { get; set; }

        [XmlIgnore]
        public DateTime dhEvento { get; set; }

        [XmlElement(ElementName = "dhEvento")]
        public string ProxydhEvento
        {
            get { return dhEvento.ToUtcString(); }
            set { dhEvento = DateTime.Parse(value); }
        }

        public eNFeTipoEvento tpEvento { get; set; }

        public int nSeqEvento { get; set; }

        public string verEvento { get; set; }

        public detEvento detEvento { get; set; }
    }
}