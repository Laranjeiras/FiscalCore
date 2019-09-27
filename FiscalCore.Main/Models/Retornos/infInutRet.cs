using FiscalCore.Main.Enums;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Retornos
{
    public class infInutRet
    {
        [XmlAttribute]
        public string Id { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public string verAplic { get; set; }

        public int cStat { get; set; }

        public string xMotivo { get; set; }

        public eUF? cUF { get; set; }

        public int? ano { get; set; }

        public string CNPJ { get; set; }

        public eModeloDocumento? mod { get; set; }

        public int? serie { get; set; }
        
        public int? nNFIni { get; set; }

        public int? nNFFin { get; set; }

        public DateTime? dhRecbto { get; set; }

        public string nProt { get; set; }
    }
}