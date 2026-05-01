#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

#endregion

using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal
{
    public class PISNT : PISBasico
    {
        #region Propriedades

        /// <summary>
        ///     Q06 - Código de Situação Tributária do PIS
        /// </summary>
        public CSTPIS CST { get; set; }

        #endregion
    }
}