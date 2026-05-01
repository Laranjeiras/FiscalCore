#nullable disable
#pragma warning disable CS8981
using FiscalCore.Utils;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal
{
    public class COFINSOutr : COFINSBasico
    {
        #region Propriedades

        /// <summary>
        ///     S06 - Código de Situação Tributária da COFINS
        /// </summary>
        public CSTCOFINS CST { get; set; }

        /// <summary>
        ///     S07 - Valor da Base de Cálculo da COFINS
        /// </summary>
        public decimal? vBC
        {
            get { return _vBc.Arredondar(2); }
            set { _vBc = value.Arredondar(2); }
        }

        /// <summary>
        ///     S08 - Alíquota da COFINS (em percentual)
        /// </summary>
        public decimal? pCOFINS
        {
            get { return _pCofins.Arredondar(4); }
            set { _pCofins = value.Arredondar(4); }
        }

        /// <summary>
        ///     S09 - Quantidade Vendida
        /// </summary>
        public decimal? qBCProd
        {
            get { return _qBcProd.Arredondar(4); }
            set { _qBcProd = value.Arredondar(4); }
        }

        /// <summary>
        ///     S10 - Alíquota da COFINS (em reais)
        /// </summary>
        public decimal? vAliqProd
        {
            get { return _vAliqProd.Arredondar(4); }
            set { _vAliqProd = value.Arredondar(4); }
        }

        /// <summary>
        ///     S11 - Valor da COFINS
        /// </summary>
        public decimal? vCOFINS
        {
            get { return _vCofins.Arredondar(2); }
            set { _vCofins = value.Arredondar(2); }
        }

        #endregion

        #region Variaveis Globais

        private decimal? _pCofins;
        private decimal? _qBcProd;
        private decimal? _vAliqProd;
        private decimal? _vBc;
        private decimal? _vCofins;

        #endregion

        public bool ShouldSerializevBC()
        {
            return vBC.HasValue;
        }

        public bool ShouldSerializepCOFINS()
        {
            return pCOFINS.HasValue;
        }

        public bool ShouldSerializeqBCProd()
        {
            return qBCProd.HasValue;
        }

        public bool ShouldSerializevAliqProd()
        {
            return vAliqProd.HasValue;
        }

        public bool ShouldSerializevCOFINS()
        {
            return vCOFINS.HasValue;
        }
    }
}