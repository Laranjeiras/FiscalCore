#nullable disable
#pragma warning disable CS8981
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FiscalCore.Tipos;

namespace FiscalCore.Modelos.Eventos
{
    public class detEvento
    {
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     HP19 - Informar a descrição do evento
        /// </summary>
        public string descEvento { get; set; }

        #region Cancelamento

        /// <summary>
        ///     HP20 - Informar o número do Protocolo de Autorização da NF-e a ser Cancelada.
        /// </summary>
        public string nProt { get; set; }

        /// <summary>
        ///     HP21 - Informar a justificativa do cancelamento
        /// </summary>
        public string xJust { get; set; }

        #endregion

        #region Carta de Correção
        /// <summary>
        ///     HP20 - Correção a ser considerada, texto livre. A correção mais recente substitui as anteriores.
        /// </summary>
        public string xCorrecao { get; set; }

        /// <summary>
        ///     HP20a - Condições de uso da Carta de Correção
        /// </summary>
        public string xCondUso
        {
            set {  }
            get
            {
                if (descricoes.Where(x => x == descEvento).Any())
                    return "A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.";
                else
                    return null;
            }
        }

        private IList<string> descricoes => new List<string> { "Carta de Correção", "Carta de Correcao" };
        #endregion

        #region Eventos RTC
        public eUF? cOrgaoAutor { get; set; }
        public bool ShouldSerializecOrgaoAutor() => cOrgaoAutor.HasValue;

        public string tpAutor { get; set; }
        public bool ShouldSerializetpAutor() => !string.IsNullOrEmpty(tpAutor);

        public string verAplic { get; set; }
        public bool ShouldSerializeverAplic() => !string.IsNullOrEmpty(verAplic);

        public string indAceitacao { get; set; }
        public bool ShouldSerializeindAceitacao() => !string.IsNullOrEmpty(indAceitacao);

        [XmlElement("gImobilizacao")]
        public List<gImobilizacao> gImobilizacao { get; set; }
        public bool ShouldSerializegImobilizacao() => gImobilizacao != null && gImobilizacao.Count > 0;
        #endregion
    }
}
