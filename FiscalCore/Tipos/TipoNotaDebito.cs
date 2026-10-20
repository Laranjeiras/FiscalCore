using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoNotaDebito
    {
        [XmlEnum("01")]
        TransferenciaCreditosCooperativas = 1,
        [XmlEnum("02")]
        AnulacaoCreditoSaidasImunesIsentas = 2,
        [XmlEnum("03")]
        DebitosNotasNaoProcessadasApuracao = 3,
        [XmlEnum("04")]
        MultaEJuros = 4,
        [XmlEnum("05")]
        TransferenciaCreditoSucessao = 5,
        [XmlEnum("06")]
        PagamentoAntecipado = 6,
        [XmlEnum("07")]
        PerdaEstoque = 7,
        [XmlEnum("08")]
        DesenquadramentoSN = 8
    }
}
