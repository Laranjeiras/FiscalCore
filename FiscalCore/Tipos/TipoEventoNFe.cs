using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoEventoNFe
    {
        [Description("Carta de Correção")]
        [XmlEnum("110110")]
        NFeCartaCorrecao = 110110,

        [Description("EPEC")]
        [XmlEnum("110140")]
        NFceEpec = 110140,

        [Description("Cancelamento")]
        [XmlEnum("110111")]
        NFeCancelamento = 110111,

        [Description("Cancelamento por substituicao")]
        [XmlEnum("110112")]
        NFeCancelamentoSubstituicao = 110112,
    }
}
