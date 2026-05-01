#nullable disable
#pragma warning disable CS8981
using FiscalCore.Tipos;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    public class consNFeDest
    {
        [XmlAttribute("versao")]
        public string Versao { get; set; }

        [XmlElement("tpAmb")]
        public eTipoAmbiente TpAmb { get; set; }

        [XmlElement("xServ")]
        public string XServ { get; set; }

        [XmlElement("CNPJ")]
        public string Cnpj { get; set; }

        [XmlElement("indNFe")]
        public int IndNFe { get; set; }

        [XmlElement("indEmi")]
        public int IndEmi { get; set; }

        [XmlElement("ultNSU")]
        public string UltNSU { get; set; }
    }
}
