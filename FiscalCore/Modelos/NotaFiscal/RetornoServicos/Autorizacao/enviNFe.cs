#nullable disable
#pragma warning disable CS8981
using System.Collections.Generic;
using System.Xml.Serialization;
using FiscalCore.Tipos;

namespace FiscalCore.NotaFiscal.RetornoServicos.Autorizacao
{
    [XmlRoot(ElementName = "enviNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class enviNFe
    {
        #region Propriedades

        /// <summary>
        ///     AP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     AP03 - Identificador de controle do envio do lote.
        /// </summary>
        public int idLote { get; set; }

        /// <summary>
        ///     AP03a - Indicador de Sincronização
        /// </summary>
        public eIndicadorSincronizacao indSinc { get; set; }

        /// <summary>
        ///     AP04 - Conjunto de NF-e transmitidas
        /// </summary>
        [XmlElement("NFe")]
        public List<NFe> NFe { get; set; }

        #endregion

        #region Construtor

        public enviNFe(string versao, int idLote, eIndicadorSincronizacao indSinc, List<NFe> nFe)
        {
            this.versao = versao;
            this.idLote = idLote;
            this.indSinc = indSinc;
            NFe = nFe;
        }

        internal enviNFe() //para serialização apenas
        {
        }

        #endregion
    }
}