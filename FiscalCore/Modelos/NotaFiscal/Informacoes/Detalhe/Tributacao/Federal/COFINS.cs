using FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using System.Xml.Serialization;


namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Federal
{
    public class COFINS
    {
        #region Propriedades

        /// <summary>
        ///     <para>S02 (COFINSAliq) - Grupo COFINS tributado pela alíquota</para>
        ///     <para>S03 (COFINSQtde) - Grupo COFINS tributado por Qtde</para>
        ///     <para>S04 (COFINSNT) - Grupo COFINS não tributado</para>
        ///     <para>S05 (COFINSOutr) - Grupo COFINS Outras Operações</para>
        /// </summary>
        [XmlElement("COFINSAliq", typeof(COFINSAliq))]
        [XmlElement("COFINSQtde", typeof(COFINSQtde))]
        [XmlElement("COFINSNT", typeof(COFINSNT))]
        [XmlElement("COFINSOutr", typeof(COFINSOutr))]
        public COFINSBasico TipoCOFINS { get; set; }

        #endregion
    }
}