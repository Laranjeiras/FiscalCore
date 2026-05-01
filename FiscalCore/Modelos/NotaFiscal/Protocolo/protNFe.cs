#nullable disable
#pragma warning disable CS8981
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.Protocolo
{
    public class protNFe
    {
        /// <summary>
        ///     PR02 - Versão do leiaute das informações de Protocolo.
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     PR03 - Informações do Protocolo de resposta. TAG a ser assinada
        /// </summary>
        [XmlElement("infProt")]
        public infProt infProt { get; set; }

        public protNFe()
        {
            //infProt = new List<infProt>();
        }
    }
}