using FiscalCore.ValueObjects;
using Xunit;

namespace FiscalCore.Testes.ValueObjects;

/// <summary>
/// CNPJ alfanumérico — IN RFB nº 2.119/2022, Anexo XV.
/// Posições 1–12 aceitam [A-Z0-9]; posições 13–14 (DVs) são sempre numéricas.
/// </summary>
public sealed class CnpjTests
{
    [Theory]
    // Alfanuméricos — "12ABC34501DE" tem DV 35 (exemplo oficial da IN RFB)
    [InlineData("12.ABC.345/01DE-35", true)]
    [InlineData("12ABC34501DE35", true)]
    [InlineData("12abc34501de35", true)]
    // Numéricos legados — continuam válidos após jul/2026
    [InlineData("11.222.333/0001-81", true)]
    [InlineData("11222333000181", true)]
    // DV incorreto
    [InlineData("12ABC34501DE34", false)]
    [InlineData("12ABC34501DE00", false)]
    [InlineData("11222333000180", false)]
    // Sequências homogêneas: DV correto, mas reprovadas por regra
    [InlineData("00000000000000", false)]
    [InlineData("AAAAAAAAAAAA45", false)]
    // DV precisa ser numérico
    [InlineData("12ABC34501DEA5", false)]
    [InlineData("12ABC34501DE3A", false)]
    // Comprimento
    [InlineData("12ABC34501DE3", false)]
    [InlineData("12ABC34501DE355", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    // Caracteres fora de [A-Z0-9] nas posições 1–12
    [InlineData("12@BC34501DE35", false)]
    public void IsValid_DeveAvaliarCnpjAlfanumericoENumerico(string? cnpj, bool esperado)
        => Assert.Equal(esperado, Cnpj.IsValid(cnpj));

    [Theory]
    [InlineData("12.ABC.345/01DE-35", true)]
    [InlineData("11.222.333/0001-81", true)]
    [InlineData("12ABC34501DE00", false)]
    [InlineData(null, false)]
    public void Valido_DeveRefletirIsValid(string? cnpj, bool esperado)
        => Assert.Equal(esperado, new Cnpj(cnpj!).Valido);

    [Theory]
    [InlineData("12.ABC.345/01DE-35", "12ABC34501DE35")]
    [InlineData("12abc34501de35", "12ABC34501DE35")]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    public void Value_DeveRemoverMascaraENormalizarParaMaiusculas(string entrada, string esperado)
        => Assert.Equal(esperado, new Cnpj(entrada).Value);

    [Theory]
    [InlineData("12ABC34501DE35", true)]
    [InlineData("12abc34501de35", true)]
    [InlineData("11222333000181", false)]
    public void Alfanumerico_DeveIndicarPresencaDeLetras(string cnpj, bool esperado)
        => Assert.Equal(esperado, new Cnpj(cnpj).Alfanumerico);

    [Fact]
    public void OperadorImplicito_DeveConverterDeString()
    {
        Cnpj cnpj = "12.ABC.345/01DE-35";

        Assert.True(cnpj.Valido);
        Assert.Equal("12ABC34501DE35", cnpj.Value);
    }

    [Fact]
    public void ValidarCNPJ_DeveDelegarParaValido()
    {
        Assert.True(Cnpj.ValidarCNPJ(new Cnpj("12ABC34501DE35")));
        Assert.False(Cnpj.ValidarCNPJ(new Cnpj("12ABC34501DE00")));
    }

    [Fact]
    public void ToString_DevePreservarEntradaOriginal()
        => Assert.Equal("12.ABC.345/01DE-35", new Cnpj("12.ABC.345/01DE-35").ToString());

    [Fact]
    public void Cnpj_ComValorNulo_NaoDeveLancar()
    {
        var cnpj = new Cnpj(null!);

        Assert.False(cnpj.Valido);
    }

    /// <summary>
    /// O valor chega de XML e de certificado, fora do controle desta camada. O buffer
    /// interno é dimensionado pelo comprimento de um CNPJ, não pelo da entrada — caso
    /// contrário uma entrada longa derrubaria o processo com StackOverflowException,
    /// que não é capturável.
    /// </summary>
    [Fact]
    public void IsValid_ComEntradaExcessivamenteLonga_DeveRejeitarSemEstourarAPilha()
    {
        Assert.False(Cnpj.IsValid(new string('1', 5_000_000)));
        Assert.True(Cnpj.IsValid(new string('.', 5_000_000) + "12ABC34501DE35"));
    }
}
