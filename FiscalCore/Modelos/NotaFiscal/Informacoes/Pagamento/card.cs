#nullable disable
#pragma warning disable CS8981
namespace FiscalCore.NotaFiscal.Informacoes.Pagamento
{
    public class card
    {
        #region Propriedades

        /// <summary>
        ///     YA04a - Tipo de Integração para pagamento
        /// </summary>
        public TipoIntegracaoPagamento? tpIntegra { get; set; }

        /// <summary>
        ///     YA05 - CNPJ da Credenciadora de cartão de crédito e/ou débito
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///     YA06 - Bandeira da operadora de cartão de crédito e/ou débito
        /// </summary>
        public BandeiraCartao? tBand { get; set; }

        /// <summary>
        ///     YA07 - Número de autorização da operação cartão de crédito e/ou débito
        /// </summary>
        public string cAut { get; set; }

        #endregion

        public bool ShouldSerializetpIntegra()
        {
            return tpIntegra.HasValue;
        }

        public bool ShouldSerializetBand()
        {
            return tBand.HasValue;
        }
    }
}