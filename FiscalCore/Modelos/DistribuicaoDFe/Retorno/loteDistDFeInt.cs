using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]    
    public class loteDistDFeInt
    {
        /// <summary>
        /// B12 - NSU do documento fiscal
        /// </summary>
        [XmlAttribute()]
        public long NSU { get; set; }

        /// <summary>
        /// B13 - Identificação do Schema XML que será utilizado para validar o XML existente no campo seguinte.
        /// Vai identificar o tipo do documento e sua versão.
        /// Exemplos:
        /// - resNFe_v1.00.xsd
        /// - procNFe_v3.10.xsd
        /// - resEvento_1.00.xsd
        /// - procEventoNFe_v1.00.xsd
        /// </summary>
        [XmlAttribute()]
        public string schema { get; set; }

        /// <summary>
        /// Conteudo da Tag schema. 
        /// O conteúdo desta tag estará compactado no padrão gZip.O tipo do campo é base64Binary.
        /// </summary>
        [XmlText(DataType = "base64Binary")]
        public byte[] XmlNfe { get; set; }

        ///// <summary>
        ///// Resumo de NF-e 
        ///// </summary>
        //[XmlIgnore]
        //public resNFe ResNFe { get; set; }

        /// <summary>
        /// Eventos da NFe
        /// </summary>
        [XmlIgnore]
        public procEventoNFe ProcEventoNFe { get; set; }

        /// <summary>
        /// Resumos de eventos
        /// </summary>
        [XmlIgnore]
        public resEvento ResEvento { get; set; }

        ///// <summary>
        ///// NF-e processada
        ///// </summary>
        //[XmlIgnore]
        //public FiscalCore.NotaFiscal.nfeProc NfeProc { get; set; }
    }
}