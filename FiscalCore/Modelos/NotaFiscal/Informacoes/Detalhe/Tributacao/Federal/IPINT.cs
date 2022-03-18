
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal
{
    public class IPINT : IPIBasico
    {
        #region Propriedades

        /// <summary>
        ///     O09 - Código da Situação Tributária do IPI:
        /// </summary>
        public CSTIPI CST { get; set; }

        #endregion
    }
}