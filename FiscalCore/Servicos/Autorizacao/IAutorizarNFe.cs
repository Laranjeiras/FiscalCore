using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Modelos.Retornos;
using System.Collections.Generic;

namespace FiscalCore.Servicos
{
    public interface IAutorizarNFe
    {
        IRetornoAutorizacao Autorizar(NFe nfe, int idLote = 0);
        IRetornoAutorizacao Autorizar(IList<NFe> nfe, int idLote = 0);
    }
}
