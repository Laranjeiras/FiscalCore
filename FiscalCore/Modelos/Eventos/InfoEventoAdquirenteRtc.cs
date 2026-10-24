#nullable disable
using System.Collections.Generic;
using FiscalCore.Tipos;

namespace FiscalCore.Modelos.Eventos
{
    public class InfoEventoAdquirenteRtc
    {
        public string ChaveAcesso { get; set; }
        public int nSeqEvento { get; set; }
        public eTipoEventoNFe TipoEvento { get; set; }
        public string IndAceitacao { get; set; }
        public List<gImobilizacao> ItensImobilizados { get; set; }
        public string VersaoAplicativo { get; set; }
        public int IdLote { get; set; } = 1;
    }
}
