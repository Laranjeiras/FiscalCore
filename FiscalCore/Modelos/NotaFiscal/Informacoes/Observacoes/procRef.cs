#nullable disable
#pragma warning disable CS8981



// Autores: 







namespace FiscalCore.NotaFiscal.Informacoes.Observacoes
{
    public class procRef
    {
        #region Propriedades

        /// <summary>
        ///     Z11 - Identificador do processo ou ato concessório
        /// </summary>
        public string nProc { get; set; }

        /// <summary>
        ///     Z12 - Indicador da origem do processo
        /// </summary>
        public IndicadorProcesso indProc { get; set; }

        #endregion
    }
}