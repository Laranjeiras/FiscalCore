using FiscalCore.Modelos.Retornos;
using FiscalCore.NotaFiscal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos
{
    public interface IAutorizarNFeServico
    {
        Task<IRetornoAutorizacao> Autorizar(NFe nfe, CancellationToken cancellation, int idLote = 0);
        Task<IRetornoAutorizacao> Autorizar(IList<NFe> nfe, CancellationToken cancellation, int idLote = 0);
    }
}
