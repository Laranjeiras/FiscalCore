#nullable disable
#pragma warning disable CS8981



// Autores: 







namespace FiscalCore.NotaFiscal.Informacoes.Total
{
    public class total
    {
        #region Propriedades

        /// <summary>
        ///     W02 - Grupo Totais referentes ao ICMS
        /// </summary>
        public ICMSTot ICMSTot { get; set; }

        /// <summary>
        ///     W17 - Grupo Totais referentes ao ISSQN
        /// </summary>
        public ISSQNtot ISSQNtot { get; set; }

        /// <summary>
        ///     W23 - Grupo Retenções de Tributos
        /// </summary>
        public retTrib retTrib { get; set; }

        /// <summary>
        ///     W34 - Totais do IBS e da CBS (Reforma Tributária do Consumo). Nulo = omitido no XML.
        /// </summary>
        public IBSCBSTot IBSCBSTot { get; set; }

        #endregion
    }
}