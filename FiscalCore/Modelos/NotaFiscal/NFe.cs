using FiscalCore.Modelos.Signatures;
using FiscalCore.NotaFiscal.Informacoes;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class NFe
    {
        #region Propriedades

        /// <summary>
        ///     A01 - Informações da Nota Fiscal Eletrônica
        /// </summary>
        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public infNFe infNFe { get; set; }

        /// <summary>
        ///     ZX01 - Informações suplementares da Nota Fiscal
        /// </summary>
        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public infNFeSupl infNFeSupl { get; set; }

        /// <summary>
        ///     XS01
        /// </summary>
        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }

        #endregion
    }
}