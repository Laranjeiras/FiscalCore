using System.ComponentModel;

namespace FiscalCore.Tipos
{
    public enum eModalidadeFrete
    {
        [Description("Por Conta Emitente")]
        PorContaEmitente = 0,
        [Description("Por Conta Destinatario")]
        PorContaDestinatario = 1,
        [Description("Por Conta Terceiros")]
        PorContaTerceiros = 2,
        [Description("Proprio Conta Remente")]
        ProprioContaRemente = 3,
        [Description("Proprio Conta Destinatario")]
        ProprioContaDestinatario = 4,
        [Description("Sem Frete")]
        SemFrete = 9
    }
}
