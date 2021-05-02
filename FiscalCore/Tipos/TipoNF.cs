using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    /// <summary>
    ///     Tipo do Documento Fiscal (0 - entrada; 1 - saída)
    /// </summary>
    public enum eTipoNF
    {
        [Description("Entrada")]
        [XmlEnum("0")]
        Entrada = 0,

        [Description("Saída")]
        [XmlEnum("1")]
        Saida = 1
    }
}
