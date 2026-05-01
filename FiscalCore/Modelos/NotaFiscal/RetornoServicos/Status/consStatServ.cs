#nullable disable
#pragma warning disable CS8981



// Autores: 







#region

using System.Xml.Serialization;
using FiscalCore.Tipos;

#endregion

namespace FiscalCore.NotaFiscal.RetornoServicos.Status
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class consStatServ
    {
        #region Propriedades

        /// <summary>
        ///     FP02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     FP03 - Identificação do Ambiente: 1 – Produção / 2 - Homologação
        /// </summary>
        public eTipoAmbiente tpAmb { get; set; }

        /// <summary>
        ///     FP04 - Código da UF consultada
        /// </summary>
        public eUF cUF { get; set; }

        /// <summary>
        ///     Serviço solicitado 'STATUS'
        /// </summary>
        public string xServ { get; set; }

        #endregion

        #region Construtor

        public consStatServ()
        {
            xServ = "STATUS";
        }

        #endregion
    }
}