using FiscalCore.Tipos;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Retornos
{
    public class infEventoRet
    {
        [XmlAttribute]
        public string Id { get; set; }
        public eTipoAmbiente tpAmb { get; set; }
        public string verAplic { get; set; }
        public eUF cOrgao { get; set; }
        public int cStat { get; set; }
        public string xMotivo { get; set; }
        public string chNFe { get; set; }
        public eTipoEventoNFe? tpEvento { get; set; }
        public string xEvento { get; set; }
        public int? nSeqEvento { get; set; }
        public string CnpjDest { get; set; }
        public string CpfDest { get; set; }
        public DateTime dhRegEvento { get; set; }
        public string nProt { get; set; }
    }
}