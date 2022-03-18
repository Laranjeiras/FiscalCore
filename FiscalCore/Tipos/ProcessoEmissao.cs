using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eProcessoEmissao
    {
        [XmlEnum("0")]
        [Description("0 - Aplicativo do contribuinte")]
        AplicativoContribuinte,
        [XmlEnum("1")]
        [Description("1 - Avulsa pelo Fisco")]
        AvulsaFisco,
        [XmlEnum("2")]
        [Description("2 - Avulsa pelo site do Fisco")]
        AvulsaSiteFisco,
        [XmlEnum("3")]
        [Description("3 - Aplicativo fornecido pelo Fisco")]
        AplicativoFisco
    }
}
