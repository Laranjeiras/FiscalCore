using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoEventoNFe
    {
        [Description("Carta de Correção")]
        [XmlEnum("110110")]
        NFeCartaCorrecao = 110110,

        [Description("Cancelamento")]
        [XmlEnum("110111")]
        NFeCancelamento = 110111,

        [Description("Cancelamento por substituicao")]
        [XmlEnum("110112")]
        NFeCancelamentoSubstituicao = 110112,

        [Description("EPEC")]
        [XmlEnum("110140")]
        NFceEpec = 110140,

        [Description("Confirmação da Operação")]
        [XmlEnum("210200")]
        ConfirmacaoOperacao = 210200,

        [Description("Ciência da Operação")]
        [XmlEnum("210210")]
        CienciaOperacao = 210210,

        [Description("Desconhecimento da Operação")]
        [XmlEnum("210220")]
        DesconhecimentoOperacao = 210220,

        [Description("Operação não Realizada")]
        [XmlEnum("210240")]
        OperacaoNaoRealizada = 210240
    }
}
