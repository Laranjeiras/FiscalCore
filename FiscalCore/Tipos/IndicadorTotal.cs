using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    /// <summary>
    ///     <para>0 – O valor do item (vProd) não compõe o valor total da NF-e (vProd)</para>
    ///     <para>1 – O valor do item (vProd) compõe o valor total da NF-e (vProd)</para>
    /// </summary>
    public enum eIndicadorTotal
    {
        [XmlEnum("0")]
        [Description("Valor do item não compõe o total da NF")]
        ValorDoItemNaoCompoeTotalNF = 0,
        [XmlEnum("1")]
        [Description("Valor do item compõe o total da NF")]
        ValorDoItemCompoeTotalNF = 1
    }
}
