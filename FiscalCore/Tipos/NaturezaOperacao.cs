using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eNaturezaOperacao
    {
        [Description("Venda")]
        [XmlEnum("Venda")]
        Venda = 1,
        [Description("Compra")]
        [XmlEnum("Compra")]
        Compra = 2,
        [Description("Remessa para Venda Fora do Estabelecimento")]
        [XmlEnum("Remessa para Venda Fora do Estabelecimento")]
        RemessaParaVendaForaEstabelecimento = 3,
        [Description("Retorno de Remessa para Venda Fora do Estabelecimento")]
        [XmlEnum("Retorno de Remessa para Venda Fora do Estabelecimento")]
        RetornodeRemessaParaVendaForaEstabelecimento = 4,
        [Description("Devolução")]
        [XmlEnum("Devolução")]
        Devolucao = 5,
        [Description("Retorno")]
        [XmlEnum("Retorno")]
        Retorno = 6,
        [Description("Simples Remessa")]
        [XmlEnum("Simples Remessa")]
        SimplesRemessa = 7
    }
}
