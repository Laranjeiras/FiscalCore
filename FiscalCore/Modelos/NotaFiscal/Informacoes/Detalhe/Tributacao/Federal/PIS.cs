using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal
{
    public class PIS
    {
        #region Propriedades

        /// <summary>
        ///     <para>Q02 (PISAliq) - Grupo PIS tributado pela alíquota</para>
        ///     <para>Q03 (PISQtde) - Grupo PIS tributado por Qtde</para>
        ///     <para>Q04 (PISNT) - Grupo PIS não tributado</para>
        ///     <para>Q05 (PISOutr) - Grupo PIS Outras Operações</para>
        /// </summary>
        [XmlElement("PISAliq", typeof(PISAliq))]
        [XmlElement("PISQtde", typeof(PISQtde))]
        [XmlElement("PISNT", typeof(PISNT))]
        [XmlElement("PISOutr", typeof(PISOutr))]
        public PISBasico TipoPIS { get; set; }

        #endregion
    }
}