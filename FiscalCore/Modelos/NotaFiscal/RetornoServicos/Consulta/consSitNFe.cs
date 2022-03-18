


// Autores: 







#region

using System.Xml.Serialization;
using FiscalCore.Tipos;

#endregion

namespace FiscalCore.NotaFiscal.RetornoServicos.Consulta
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class consSitNFe
    {
        #region Propriedades

        /// <summary>
        ///     EP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     EP03 - Identificação do Ambiente: 1 – Produção / 2 - Homologação
        /// </summary>
        public eTipoAmbiente tpAmb { get; set; }

        /// <summary>
        ///     Serviço solicitado "CONSULTAR"
        /// </summary>
        public string xServ { get; set; }

        /// <summary>
        ///     EP05 - Chave de Acesso da NF-e.
        /// </summary>
        public string chNFe { get; set; }

        #endregion

        #region Construtor

        public consSitNFe()
        {
            xServ = "CONSULTAR";
        }

        #endregion
    }
}