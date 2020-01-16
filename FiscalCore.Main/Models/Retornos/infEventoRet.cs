using FiscalCore.Main.Enums;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Main.Models.Retornos
{
    public class infEventoRet
    {
        [XmlAttribute]
        public string Id { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public eUF cOrgao { get; set; }

        public int cStat { get; set; }

        public string xMotivo { get; set; }

        public string chNFe { get; set; }
        public eNFeTipoEvento? tpEvento { get; set; }
        public int? nSeqEvento { get; set; }

        public DateTime dhRegEvento { get; set; }

        public string nProt { get; set; }
    }
}