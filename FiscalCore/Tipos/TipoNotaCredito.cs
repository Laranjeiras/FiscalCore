using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eTipoNotaCredito
    {
        [XmlEnum("01")]
        MultaEJuros = 1,
        [XmlEnum("02")]
        ApropriacaoCreditoPresumidoZFM = 2,
        [XmlEnum("03")]
        RetornoRecusaTotalEntrega = 3,
        [XmlEnum("04")]
        ReducaoValores = 4,
        [XmlEnum("05")]
        TransferenciaCreditoSucessao = 5,
        [XmlEnum("06")]
        RetornoRecusaParcialEntrega = 6
    }
}
