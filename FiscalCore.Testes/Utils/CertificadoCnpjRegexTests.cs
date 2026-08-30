using FiscalCore.ValueObjects;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace FiscalCore.Testes.Utils;

/// <summary>
/// <c>Certificado.ExtrairCNPJArquivo</c> varre a extensão SAN do certificado com um
/// regex e confirma o candidato pelo dígito verificador. Construir um X509Certificate2
/// com SAN arbitrário foge do escopo destes testes, então aqui se exercita o par
/// regex + validação — que é onde estava o defeito: o padrão anterior, <c>\d{14}</c>,
/// não reconhecia CNPJ alfanumérico.
/// </summary>
public sealed class CertificadoCnpjRegexTests
{
    private const string PadraoCnpj = @"(?<![A-Z0-9])[A-Z0-9]{12}\d{2}(?![A-Z0-9])";

    private static string? ExtrairCnpj(string textoSan)
        => Regex.Matches(textoSan, PadraoCnpj)
            .FirstOrDefault(m => new Cnpj(m.Value).Valido)
            ?.Value;

    [Theory]
    [InlineData("CN=EMPRESA TESTE:11222333000181", "11222333000181")]
    [InlineData("CN=EMPRESA TESTE:12ABC34501DE35", "12ABC34501DE35")]
    public void ExtrairCnpj_DeveReconhecerNumericoEAlfanumerico(string san, string esperado)
        => Assert.Equal(esperado, ExtrairCnpj(san));

    [Fact]
    public void ExtrairCnpj_DeveIgnorarCandidatoComDigitoVerificadorInvalido()
        => Assert.Null(ExtrairCnpj("CN=EMPRESA TESTE:11222333000180"));

    [Fact]
    public void ExtrairCnpj_DeveIgnorarSequenciaMaiorQueCatorze()
        => Assert.Null(ExtrairCnpj("SERIAL=112223330001811234"));

    [Fact]
    public void ExtrairCnpj_DentreVariosCandidatos_DeveRetornarOValido()
        => Assert.Equal(
            "12ABC34501DE35",
            ExtrairCnpj("OU=00000000000000, CN=EMPRESA:12ABC34501DE35, OU=AC TESTE"));

    [Fact]
    public void ExtrairCnpj_SemCandidato_DeveRetornarNulo()
        => Assert.Null(ExtrairCnpj("CN=EMPRESA TESTE SEM DOCUMENTO"));
}
