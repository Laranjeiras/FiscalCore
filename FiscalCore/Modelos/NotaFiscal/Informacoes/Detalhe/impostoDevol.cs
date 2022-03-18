


// Autores: 







#region

#endregion

using FiscalCore.Utils;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe
{
    public class impostoDevol
    {
        /// <summary>
        ///     UA02 - Percentual da mercadoria devolvida
        /// </summary>
        public decimal pDevol
        {
            get { return _pDevol; }
            set { _pDevol = value.Arredondar(2); }
        }

        /// <summary>
        ///     UA03 - Informação do IPI devolvido
        /// </summary>
        public IPIDevolvido IPI { get; set; }

        private decimal _pDevol;
    }
}