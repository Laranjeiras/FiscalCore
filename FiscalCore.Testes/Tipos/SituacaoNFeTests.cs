using FiscalCore.Tipos;
using System;
using System.Linq;
using Xunit;

namespace FiscalCore.Testes.Tipos;

/// <summary>
/// Campo C14 (<c>cSitNFe</c>) do resumo da Distribuição de DFe:
/// 1=Uso autorizado, 2=Uso denegado, 3=Cancelada.
/// </summary>
public sealed class SituacaoNFeTests
{
    [Theory]
    [InlineData(eSituacaoNFe.UsoAutorizado, 1)]
    [InlineData(eSituacaoNFe.UsoDenegado, 2)]
    [InlineData(eSituacaoNFe.NFeCancelada, 3)]
    public void SituacaoNFe_DeveManterCodigoDoCampoC14(eSituacaoNFe situacao, int esperado)
        => Assert.Equal(esperado, (int)situacao);

    /// <summary>
    /// Trava a regressão: <c>UsoDenegado</c> e <c>NFeCancelada</c> compartilhavam o
    /// valor 3, o que tornava um dos dois inalcançável em switch e em ToString().
    /// </summary>
    [Fact]
    public void SituacaoNFe_NaoDeveTerValoresDuplicados()
    {
        var valores = Enum.GetValues<eSituacaoNFe>().Select(s => (int)s).ToArray();

        Assert.Equal(valores.Length, valores.Distinct().Count());
    }

    [Theory]
    [InlineData(eSituacaoNFe.UsoAutorizado, nameof(eSituacaoNFe.UsoAutorizado))]
    [InlineData(eSituacaoNFe.UsoDenegado, nameof(eSituacaoNFe.UsoDenegado))]
    [InlineData(eSituacaoNFe.NFeCancelada, nameof(eSituacaoNFe.NFeCancelada))]
    public void ToString_DeveRetornarONomeDoProprioMembro(eSituacaoNFe situacao, string esperado)
        => Assert.Equal(esperado, situacao.ToString());
}
