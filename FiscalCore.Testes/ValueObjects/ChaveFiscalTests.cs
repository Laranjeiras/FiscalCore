using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using Xunit;

namespace FiscalCore.Testes.ValueObjects;

public sealed class ChaveFiscalTests
{
    /// <summary>
    /// UF 33 (RJ), emissão 2025-08, CNPJ 11222333000181, modelo 55, série 001,
    /// número 123, emissão normal, cNF 12345678 — DV 3.
    /// </summary>
    private const string ChaveValida = "33250811222333000181550010000001231123456783";

    [Fact]
    public void Construtor_ComChaveValida_DeveExtrairTodosOsCampos()
    {
        var chave = new ChaveFiscal(ChaveValida);

        Assert.Equal(eUF.RJ, chave.UF);
        Assert.Equal("2508", chave.AnoMesEmissao);
        Assert.Equal("11222333000181", chave.Cnpj.ToString());
        Assert.Equal(eModeloDocumento.NFe, chave.Modelo);
        Assert.Equal(1, chave.Serie);
        Assert.Equal(123, chave.Numero);
        Assert.Equal(eTipoEmissao.Normal, chave.TipoEmissao);
        Assert.Equal("12345678", chave.CNF.Value);
        Assert.Equal(3, chave.DigitoVerificador);
    }

    [Fact]
    public void Construtor_ComChaveValida_DeveReconstruirAMesmaChave()
        => Assert.Equal(ChaveValida, new ChaveFiscal(ChaveValida).Chave);

    [Fact]
    public void Construtor_ComPrefixoNFe_DeveIgnorarOPrefixo()
        => Assert.Equal(ChaveValida, new ChaveFiscal($"NFe{ChaveValida}").Chave);

    [Fact]
    public void Completa_DeveIncluirOPrefixoNFe()
        => Assert.Equal($"NFe{ChaveValida}", new ChaveFiscal(ChaveValida).Completa);

    [Theory]
    [InlineData("3325081122233300018155001000000123112345678")]   // 43 caracteres
    [InlineData("332508112223330001815500100000012311234567833")] // 45 caracteres
    [InlineData("")]
    public void Construtor_ComComprimentoDiferenteDe44_DeveLancar(string chave)
        => Assert.Throws<Exception>(() => new ChaveFiscal(chave));

    [Fact]
    public void Construtor_ComCamposIndividuais_DeveGerarChaveEquivalente()
    {
        var chave = new ChaveFiscal(
            eUF.RJ,
            new DateTime(2025, 8, 15),
            new Cnpj("11222333000181"),
            eModeloDocumento.NFe,
            serie: 1,
            numero: 123,
            eTipoEmissao.Normal,
            new Cnf("12345678"));

        Assert.Equal(ChaveValida, chave.Chave);
    }

    [Fact]
    public void ChaveGerada_DeveTerOTamanhoDoLayout()
        => Assert.Equal(LayoutFiscal.TamanhoChaveAcesso, new ChaveFiscal(ChaveValida).Chave.Length);
}
