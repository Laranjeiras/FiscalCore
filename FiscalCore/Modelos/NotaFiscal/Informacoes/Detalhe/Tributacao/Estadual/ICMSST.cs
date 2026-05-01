#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

using System.Xml.Serialization;
using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using FiscalCore.Utils;

#endregion

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Estadual
{
    public class ICMSST : ICMSBasico
    {
        #region Propriedades

        /// <summary>
        ///     N11 - Origem da Mercadoria
        /// </summary>
        [XmlElement(Order = 1)]
        public OrigemMercadoria orig { get; set; }

        /// <summary>
        ///     N12- Situação Tributária
        /// </summary>
        [XmlElement(Order = 2)]
        public Csticms CST { get; set; }

        /// <summary>
        ///     N26 - Valor da BC do ICMS ST retido
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal vBCSTRet
        {
            get { return _vBcstRet; }
            set { _vBcstRet = value.Arredondar(2); }
        }

        /// <summary>
        ///     N27 - Valor do ICMS ST retido
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vICMSSTRet
        {
            get { return _vIcmsstRet; }
            set { _vIcmsstRet = value.Arredondar(2); }
        }

        /// <summary>
        ///     N31 - Valor da BC do ICMS ST da UF destino
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal vBCSTDest
        {
            get { return _vBcstDest; }
            set { _vBcstDest = value.Arredondar(2); }
        }

        /// <summary>
        ///     N32 - Valor do ICMS ST da UF destino
        /// </summary>
        [XmlElement(Order = 6)]
        public decimal vICMSSTDest
        {
            get { return _vIcmsstDest; }
            set { _vIcmsstDest = value.Arredondar(2); }
        }

        #endregion

        #region Variaveis Globais

        private decimal _vBcstDest;
        private decimal _vBcstRet;
        private decimal _vIcmsstDest;
        private decimal _vIcmsstRet;

        #endregion
    }
}