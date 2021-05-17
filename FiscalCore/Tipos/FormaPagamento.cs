using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eFormaPagamento
    {
        [Description("Dinheiro")]
        [XmlEnum("01")]
        Dinheiro = 1,

        [Description("Cheque")]
        [XmlEnum("02")]
        Cheque = 2,

        [Description("Cartão de Crédito")]
        [XmlEnum("03")]
        CartaoCredito = 3,

        [Description("Cartão de Débito")]
        [XmlEnum("04")]
        CartaoDebito = 4,

        [Description("Crédito Loja")]
        [XmlEnum("05")]
        CreditoLoja = 5,

        [Description("Vale Alimentação")]
        [XmlEnum("10")]
        ValeAlimentacao = 10,

        [Description("Vale Refeição")]
        [XmlEnum("11")]
        ValeRefeicao = 11,

        [Description("Vale Presente")]
        [XmlEnum("12")]
        ValePresente = 12,

        [Description("Vale Combustível")]
        [XmlEnum("13")]
        ValeCombustivel = 13,

        [Description("Duplicata Mercantil")]
        [XmlEnum("13")]
        DuplicataMercantil = 14,

        [Description("Boleto Bancário")]
        [XmlEnum("15")]
        BoletoBancario = 15, // VERSÃO 4.00

        [Description("Sem pagamento")]
        [XmlEnum("90")]
        SemPagamento = 90, // VERSÃO 4.00

        [Description("Outros")]
        [XmlEnum("99")]
        Outro = 99
    }
}
