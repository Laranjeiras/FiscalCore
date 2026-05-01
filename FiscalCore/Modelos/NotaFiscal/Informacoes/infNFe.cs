#nullable disable
#pragma warning disable CS8981
using FiscalCore.NotaFiscal.Informacoes.Cana;
using FiscalCore.NotaFiscal.Informacoes.Cobranca;
using FiscalCore.NotaFiscal.Informacoes.Destinatario;
using FiscalCore.NotaFiscal.Informacoes.Detalhe;
using FiscalCore.NotaFiscal.Informacoes.Emitente;
using FiscalCore.NotaFiscal.Informacoes.Identificacao;
using FiscalCore.NotaFiscal.Informacoes.Observacoes;
using FiscalCore.NotaFiscal.Informacoes.Pagamento;
using FiscalCore.NotaFiscal.Informacoes.RespTec;
using FiscalCore.NotaFiscal.Informacoes.Total;
using FiscalCore.NotaFiscal.Informacoes.Transporte;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.Informacoes
{
    public class infNFe
    {
        public infNFe()
        {
            det = new List<det>();
        }

        /// <summary>
        ///     A02 - Versão do leiaute da NFe (2.0, 3.1, etc)
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     A03 - Identificador da TAG a ser assinada
        ///     <para>informar a chave de acesso da NF-e precedida do literal "NFe", acrescentada a validação do formato (v2.0).</para>
        /// </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        ///     B01 - Identificação da NF-e
        /// </summary>
        public ide ide { get; set; }

        /// <summary>
        ///     C01 - Grupo de identificação do emitente da NF-e
        /// </summary>
        public emit emit { get; set; }

        /// <summary>
        ///     D01 - Informações do fisco emitente (uso exclusivo do fisco)
        /// </summary>
        public avulsa avulsa { get; set; }

        /// <summary>
        ///     E01 - Grupo de identificação do Destinatário da NF-e
        /// </summary>
        public dest dest { get; set; }

        /// <summary>
        ///     F01 - Identificação do Local de retirada
        /// </summary>
        public retirada retirada { get; set; }

        /// <summary>
        ///     G01 - Identificação do Local de entrega
        /// </summary>
        public entrega entrega { get; set; }

        /// <summary>
        ///     GA01 - Pessoas autorizadas a acessar o XML da NF-e
        ///     <para>Ocorrência: 0-10</para>
        /// </summary>
        [XmlElement("autXML")]
        public List<autXML> autXML { get; set; }

        /// <summary>
        ///     H01 - Dados dos detalhes da NF-e
        ///     <para>Ocorrência: 1-990</para>
        /// </summary>
        [XmlElement("det")]
        public List<det> det { get; set; }

        /// <summary>
        ///     W01 - Grupo Totais da NF-e
        /// </summary>
        public total total { get; set; }

        /// <summary>
        ///     X01 - Grupo Informações do Transporte
        /// </summary>
        public transp transp { get; set; }

        /// <summary>
        ///     Y01 - Grupo Cobrança
        /// </summary>
        public cobr cobr { get; set; }

        /// <summary>
        ///     YA01 - Grupo de Formas de Pagamento
        ///     <para>Ocorrência: 0-100</para>
        /// </summary>
        [XmlElement("pag")]
        public List<pag> pag { get; set; }

        /// <summary>
        ///     Z01 - Grupo de Informações Adicionais
        /// </summary>
        public infAdic infAdic { get; set; }

        /// <summary>
        ///     ZA01 - Grupo Exportação
        /// </summary>
        public exporta exporta { get; set; }

        /// <summary>
        ///     ZB01 - Grupo Compra
        /// </summary>
        public compra compra { get; set; }

        /// <summary>
        ///     ZC01 - Grupo Cana
        /// </summary>
        public cana cana { get; set; }

        /// <summary>
        ///     ZD01 - Informações do Responsável Técnico pela Emissão do DF-e
        /// </summary>
        public infRespTec infRespTec { get; set; }

    }
}