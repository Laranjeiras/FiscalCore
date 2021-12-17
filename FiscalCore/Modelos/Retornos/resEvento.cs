using FiscalCore.Tipos;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Retornos
{
    [DesignerCategory("code")]
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class resEvento
    {
        [XmlAttribute]
        public decimal versao { get; set; }
        public string cOrgao { get; set; }
        public ulong CNPJ { get; set; }
        public ulong CPF { get; set; }
        [XmlElement(DataType = "integer")]
        public string chNFe { get; set; }
        //[XmlIgnore]
        public DateTime dhEvento { get; set; }
        //[XmlElement(ElementName = "dhEvento")]
        //public string ProxydhEvento { get; set; }
        public eTipoEventoNFe tpEvento { get; set; }
        public string nSeqEvento { get; set; }
        public string xEvento { get; set; }
        public DateTime dhRecbto { get; set; }
        public ulong nProt { get; set; }
    }
}
