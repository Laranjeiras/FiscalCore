namespace FiscalCore.NotaFiscal.Informacoes.Transporte
{
    public class reboque
    {
        #region Propriedades

        /// <summary>
        ///     X23 - Placa do Veículo
        /// </summary>
        public string placa { get; set; }

        /// <summary>
        ///     X24 - Sigla da UF
        /// </summary>
        public string UF { get; set; }

        /// <summary>
        ///     X25 - Registro Nacional de Transportador de Carga (ANTT)
        /// </summary>
        public string RNTC { get; set; }

        #endregion
    }
}