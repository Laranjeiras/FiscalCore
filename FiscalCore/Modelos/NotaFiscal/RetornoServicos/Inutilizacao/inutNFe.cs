#nullable disable
#pragma warning disable CS8981
using FiscalCore.Modelos.Signatures;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.RetornoServicos.Inutilizacao
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class inutNFe
    {
        #region Propriedades

        /// <summary>
        ///     DP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     DP03 - Dados do Pedido
        ///     TAG a ser assinada
        /// </summary>
        public infInutEnv infInut { get; set; }

        /// <summary>
        ///     DP15 - Assinatura XML do grupo identificado pelo atributo “Id”
        /// </summary>
        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }

        #endregion
    }
}