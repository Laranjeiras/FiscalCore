using System;
using System.Xml.Serialization;
using FiscalCore.Extensions;
using FiscalCore.Tipos;

namespace FiscalCore.NotaFiscal.RetornoServicos.Status
{


    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class retConsStatServ  
    {


        /// <summary>
        ///     B02 - Versão do leiaute
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     B03 - Identificação do Ambiente: 1=Produção /2=Homologação
        /// </summary>
        public eTipoAmbiente tpAmb { get; set; }

        /// <summary>
        ///     B04 - Versão do aplicativo que processou a consulta
        /// </summary>
        public string verAplic { get; set; }

        /// <summary>
        ///     B05 - Código do status da resposta (vide item 5)
        /// </summary>
        public int cStat { get; set; }

        /// <summary>
        ///     B06 - Descrição literal do status da resposta
        /// </summary>
        public string xMotivo { get; set; }

        /// <summary>
        ///     B07 - Data e hora da mensagem de Resposta
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset dhRecbto { get; set; }

        [XmlElement(ElementName = "dhRecbto")]
        public string ProxydhRecbto
        {
            get { return dhRecbto.ParaDataHoraStringUtc(); }
            set { dhRecbto = DateTimeOffset.Parse(value); }
        }

        /// <summary>
        ///     FR07 - Código da UF que atendeu a solicitação
        /// </summary>
        public eUF cUF { get; set; }

        /// <summary>
        /// Tempo Médio de Respota
        /// </summary>
        public int tMed { get; set; }
         
    }
}