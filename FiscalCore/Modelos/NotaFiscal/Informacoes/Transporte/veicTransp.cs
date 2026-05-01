#nullable disable
#pragma warning disable CS8981
namespace FiscalCore.NotaFiscal.Informacoes.Transporte
{
    public class veicTransp
    {
        #region Propriedades

        /// <summary>
        ///     X19 - Placa do Veículo
        /// </summary>
        public string placa { get; set; }

        /// <summary>
        ///     X20 - Sigla da UF
        /// </summary>
        public string UF { get; set; }

        /// <summary>
        ///     X21 - Registro Nacional de Transportador de Carga (ANTT)
        /// </summary>
        public string RNTC { get; set; }

        #endregion
    }
}