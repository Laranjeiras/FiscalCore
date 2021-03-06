using FiscalCore.Enums;
using FiscalCore.Modelos.Signatures;
using System;

namespace FiscalCore.Modelos.Protocolos
{
    public class infProt
    {
        public string Id { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public string verAplic { get; set; }

        public string chNFe { get; set; }

        public DateTime dhRecbto { get; set; }
        
        public string nProt { get; set; }

        public string digVal { get; set; }

        public int cStat { get; set; }

        public string xMotivo { get; set; }

        public Signature Signature { get; set; }
    }
}