#nullable disable
#pragma warning disable CS8981
using System.Xml.Serialization;
using FiscalCore.Tipos;

namespace FiscalCore.NotaFiscal.RetornoServicos.Recepcao.Retorno
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class consReciNFe
    {
        #region Propriedades

        /// <summary>
        ///     BP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     BP03 - Identificação do Ambiente: 1 – Produção / 2 – Homologação
        /// </summary>
        public eTipoAmbiente tpAmb { get; set; }

        /// <summary>
        ///     BP04 - Número do Recibo Número gerado pelo Portal da Secretaria de Fazenda Estadual (vide item 5.5).
        /// </summary>
        public string nRec { get; set; }

        #endregion
    }
}