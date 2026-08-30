using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using Xunit;

namespace FiscalCore.Testes.Configuracoes;

/// <summary>
/// O emitente pode ser pessoa jurídica (CNPJ) ou física (CPF) — <c>emit.CpfCnpj</c>
/// resolve para <c>CNPJ ?? CPF</c>. A validação precisa aceitar ambos.
/// </summary>
public sealed class ConfiguracaoBasicaServicoTests
{
    private static ConfiguracaoBasicaServico ComEmitente(string cpfCnpj)
        => new() { CNPJEmitente = cpfCnpj };

    [Theory]
    [InlineData("12ABC34501DE35")]      // CNPJ alfanumérico
    [InlineData("12.ABC.345/01DE-35")]  // com máscara
    [InlineData("11222333000181")]      // CNPJ numérico legado
    [InlineData("11144477735")]         // CPF (emitente pessoa física)
    public void Validar_ComDocumentoValido_NaoDeveLancar(string cpfCnpj)
        => ComEmitente(cpfCnpj).Validar();

    [Theory]
    [InlineData("12ABC34501DE00")]      // CNPJ com DV errado
    [InlineData("11222333000180")]      // CNPJ numérico com DV errado
    [InlineData("00000000000000")]      // sequência homogênea
    [InlineData("11144477700")]         // CPF com DV errado
    [InlineData("00000000000")]         // CPF homogêneo
    [InlineData("123")]                 // comprimento inválido
    [InlineData("naoehumdocumento")]
    public void Validar_ComDocumentoInvalido_DeveLancar(string cpfCnpj)
        => Assert.Throws<ConfiguracaoException>(() => ComEmitente(cpfCnpj).Validar());

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validar_SemDocumento_DeveLancar(string? cpfCnpj)
        => Assert.Throws<ConfiguracaoException>(() => ComEmitente(cpfCnpj!).Validar());
}
