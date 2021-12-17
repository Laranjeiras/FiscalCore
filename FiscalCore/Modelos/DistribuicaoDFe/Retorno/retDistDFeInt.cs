using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.DistribuicaoDFe
{
    /// <summary>
    /// Estrutura XML com os documentos de interesse do ator (qtde máxima=50)
    /// </summary>
    [XmlRoot("retDistDFeInt", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class retDistDFeInt
    {
        [XmlAttribute("versao")]
        public decimal Versao { get; set; }

        public byte tpAmb { get; set; }

        public string verAplic { get; set; }

        public int cStat { get; set; }

        public string xMotivo { get; set; }

        public DateTime dhResp { get; set; }

        /// <summary>
        /// B08 - Último NSU pesquisado no Ambiente Nacional. Se for o caso, o solicitante pode continuar a consulta a partir 
        /// deste NSU para obter novos resultados.
        /// </summary>
        public long ultNSU { get; set; }

        /// <summary>
        /// B09 - Maior NSU existente no Ambiente Nacional para o CNPJ/CPF informado
        /// </summary>
        public long maxNSU { get; set; }

        /// <summary>
        /// B10 Conjunto de informações resumidas e documentos fiscais eletrônicos de interesse da pessoa física ou empresa. 
        /// </summary>
        [XmlArrayItem("docZip", IsNullable = false)]
        public loteDistDFeInt[] loteDistDFeInt { get; set; }

        public void AddLoteDistDFe(loteDistDFeInt value)
        {
            List<loteDistDFeInt> termsList = new List<loteDistDFeInt>(loteDistDFeInt);
            termsList.Add(value);

            // You can convert it back to an array if you would like to
            loteDistDFeInt = termsList.ToArray();
        }
    }
}
