using System.ComponentModel;

namespace FiscalCore.Tipos
{
    /// <summary>
    /// Situação da NF-e — campo C14 (<c>cSitNFe</c>) do resumo da Distribuição de DFe.
    /// </summary>
    public enum eSituacaoNFe
    {
        [Description("Uso Autorizado")]
        UsoAutorizado = 1,
        [Description("Uso Denegado")]
        UsoDenegado = 2,
        [Description("NFe Cancelada")]
        NFeCancelada = 3,
    }
}
