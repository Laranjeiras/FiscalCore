#nullable disable
#pragma warning disable CS8981
using FiscalCore.Utils;
using FiscalCore.Tipos;

namespace FiscalCore.NotaFiscal.Informacoes.Pagamento
{
    public class detPag
    {
        public eIndicadorPagamento? indPag { get; set; }

        public bool indPagSpecified
        {
            get { return indPag.HasValue; }
        }

        /// <summary>
        ///     YA02 - Forma de pagamento
        /// </summary>
        public eFormaPagamento tPag { get; set; }

        public decimal vPag
        {
            get { return _vPag.Arredondar(2); }
            set { _vPag = value.Arredondar(2); }
        }

        public card card { get; set; }

        private decimal _vPag;
    }
}