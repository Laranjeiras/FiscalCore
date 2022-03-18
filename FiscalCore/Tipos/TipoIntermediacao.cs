using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    /// <summary>
    ///     1=Importação por conta própria;
    ///     2=Importação por conta e ordem;
    ///     3=Importação por encomenda;
    /// </summary>
    public enum eTipoIntermediacao
    {
        /// <summary>
        ///     1=Importação por conta própria
        /// </summary>
        [XmlEnum("1")] ContaPropria = 1,

        /// <summary>
        ///     2=Importação por conta e ordem
        /// </summary>
        [XmlEnum("2")] ContaeOrdem = 2,

        /// <summary>
        ///     3=Importação por encomenda
        /// </summary>
        [XmlEnum("3")] PorEncomenda = 3
    }
}
