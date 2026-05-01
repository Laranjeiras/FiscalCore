#nullable disable
#pragma warning disable CS8981



// Autores: 







namespace FiscalCore.NotaFiscal.Informacoes
{
    public class exporta
    {
        #region Propriedades

        /// <summary>
        ///     ZA02 - Sigla da UF de Embarque ou de transposição de fronteira
        /// </summary>
        public string UFSaidaPais { get; set; }

        /// <summary>
        ///     ZA03 - Descrição do Local de Embarque ou de transposição de fronteira
        /// </summary>
        public string xLocExporta { get; set; }

        /// <summary>
        ///     ZA04 - Descrição do local de despacho
        /// </summary>
        public string xLocDespacho { get; set; }

        #endregion
    }
}