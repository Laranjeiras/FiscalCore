#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

#endregion

using FiscalCore.Utils;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe
{
    public class IPIDevolvido
    {
        #region Propriedades

        /// <summary>
        ///     UA04 - Valor do IPI devolvido
        /// </summary>
        public decimal vIPIDevol
        {
            get { return _vIpiDevol; }
            set { _vIpiDevol = value.Arredondar(2); }
        }

        #endregion

        #region Variaveis Globais

        private decimal _vIpiDevol;

        #endregion
    }
}