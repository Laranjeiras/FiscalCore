using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoImpressao
    {
        [XmlEnum("0")]
        [Description("Sem geração de DANFE")]
        SemDanfe = 0,

        [XmlEnum("1")]
        [Description("DANFE normal, Retrato")]
        NormalRetrato = 1,

        [XmlEnum("2")]
        [Description("DANFE normal, Paisagem")]
        NormalPaisagem = 2,

        [XmlEnum("3")]
        [Description("DANFE Simplificado")]
        Simplificado = 3,

        [XmlEnum("4")]
        [Description("DANFE NFC-e")]
        Nfce = 4,

        [XmlEnum("5")]
        [Description("DANFE NFC-e em mensagem eletrônica")]
        MensagemEletronica = 5
    }
}
