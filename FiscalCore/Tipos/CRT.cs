using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eCRT
    {
        [Description("Simples Nacional")]
        [XmlEnum("1")]
        SimplesNacional = 1,
        [Description("Simples Nacional – excesso de sublimite de receita bruta")]
        [XmlEnum("2")]
        SimplesNacionalExcessoSublimite = 2,
        [Description("Regime Normal")]
        [XmlEnum("3")]
        RegimeNormal = 3
    }
}
