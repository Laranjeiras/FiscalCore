using System.ComponentModel;
using System.Xml.Serialization;

namespace FiscalCore.Tipos
{
    public enum eModalidadeFrete
    {
        [Description("Por Conta Emitente")]
        [XmlEnum("0")]
        PorContaEmitente = 0,
        [Description("Por Conta Destinatario")]
        [XmlEnum("1")]
        PorContaDestinatario = 1,
        [Description("Por Conta Terceiros")]
        [XmlEnum("2")]
        PorContaTerceiros = 2,
        [Description("Proprio Conta Remente")]
        [XmlEnum("3")]
        ProprioContaRemente = 3,
        [Description("Proprio Conta Destinatario")]
        [XmlEnum("4")]
        ProprioContaDestinatario = 4,
        [Description("Sem Frete")]
        [XmlEnum("9")]
        SemFrete = 9
    }
}
