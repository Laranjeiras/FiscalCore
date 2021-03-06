using FiscalCore.Enums;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Retornos
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class retEnvEvento : BaseRetorno
    {
        [XmlAttribute]
        public string versao { get; set; } 
        public long idLote { get; set; }
        public eTipoAmbiente tpAmb { get; set; }
        public string verAplic { get; set; }
        public eUF cOrgao { get; set; }
        public int cStat { get; set; }
        public string xMotivo { get; set; }

        [XmlElement("retEvento")]
        public List<retEvento> retEvento { get; set; }
    }
}
