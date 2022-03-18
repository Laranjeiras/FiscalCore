using FiscalCore.NotaFiscal.RetornoServicos.Evento;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.RetornoServicos.Consulta
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class procEventoNFe
    {
        #region Propriedades

        /// <summary>
        ///     ZR02
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     ZR03
        /// </summary>
        [XmlElement("evento")]
        public evento Evento { get; set; }
         

        #endregion
    }
}