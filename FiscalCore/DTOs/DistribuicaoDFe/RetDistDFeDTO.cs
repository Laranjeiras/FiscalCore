using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.DistribuicaoDFe;
using System.Collections.Generic;

namespace FiscalCore.DTOs.DistribuicaoDFe
{
    public class RetDistDFeDTO
    {
        public RetDistDFeDTO()
        {
            ResNFes = new List<resNFe>();
            ProcEventos = new List<procEventoNFe>();
            NFeProcs = new List<nfeProc>();
            DistDfeIntDTOs = new List<RetDistDFeIntDTO>();
        }

        public IList<resNFe> ResNFes { get; private set; }
        public IList<procEventoNFe> ProcEventos { get; private set; }
        public IList<nfeProc> NFeProcs { get; private set; }
        public IList<RetDistDFeIntDTO> DistDfeIntDTOs { get; set; }
        public int CStat { get; set; }
        public string Motivo { get; set; }

        public bool Processado => CStat == 138;
    }
}
