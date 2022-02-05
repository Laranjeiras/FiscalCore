using System.ComponentModel;

namespace FiscalCore.Tipos
{
    public enum eSituacaoNFe
    {
        [Description("Uso Autorizado")]
        UsoAutorizado = 1,
        [Description("Uso Denegado")]
        UsoDenegado = 3,
        [Description("NFe Cancelada")]
        NFeCancelada = 3,
    }
}
