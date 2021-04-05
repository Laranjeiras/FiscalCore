using FiscalCore.Tipos;
using System;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    [XmlRoot("distDFeInt", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class distDFeInt
    {
        [XmlAttribute("versao")]
        public string Versao { get; set; }

        [XmlElement("tpAmb")]
        public eTipoAmbiente TpAmb { get; set; } = eTipoAmbiente.Producao;

        [XmlElement("cUFAutor")]
        public string cUFAutor { get; set; }

        [XmlElement("CNPJ")]
        public string Cnpj { get; set; }

        public string CPF { get; set; }

        [XmlElement("distNSU")]
        public distNSU DistNSU { get; set; }

        public consNSU consNSU { get; set; }

        /// <summary>
        ///     A11 - Grupo para consultar uma NF-e pela chave de acesso
        /// </summary>
        public consChNFe consChNFe { get; set; }
    }
}
