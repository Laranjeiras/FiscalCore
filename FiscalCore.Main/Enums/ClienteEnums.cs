using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Main.Tipos
{
    /// <summary>
    ///     Indicador da IE do Destinatário
    ///     <para>1 – Contribuinte ICMS;</para>
    ///     <para>2 – Contribuinte isento de inscrição;</para>
    ///     <para>9 – Não Contribuinte</para>
    /// </summary>
    public enum eIndIEDest
    {
        [Description("Contribuinte ICMS")]
        [XmlEnum("1")]
        ContribuinteICMS = 1,
        [Description("Contribuinte isento de inscrição")]
        [XmlEnum("2")]
        Isento = 2,
        [Description("Não Contribuinte")]
        [XmlEnum("9")]
        NaoContribuinte = 9
    }
}
