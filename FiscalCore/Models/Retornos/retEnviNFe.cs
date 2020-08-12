using FiscalCore.Enums;
using FiscalCore.Modelos.Protocolos;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Retornos
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class retEnviNFe
    {
        [XmlAttribute]
        public string versao { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public string verAplic { get; set; }

        public int cStat { get; set; }

        public string xMotivo { get; set; }

        public eUF cUF { get; set; }

        public DateTime dhRecbto { get; set; }

        public infRec infRec { get; set; }

        public protNFe protNFe { get; set; }
    }
}