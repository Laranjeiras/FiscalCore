using FiscalCore.Modelos.Retornos;
using FiscalCore.Servicos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using System.Collections.Generic;
using Xunit;

namespace FiscalCore.Testes;

public sealed class ManifestacaoDestinatarioServicoTests
{
    [Fact]
    public void ValidarItens_WhenLoteIsEmpty_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        IReadOnlyList<ManifestacaoDestinatarioItem> itens = Array.Empty<ManifestacaoDestinatarioItem>();

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => ManifestacaoDestinatarioServico.ValidarItens(itens));

        // Assert
        Assert.Equal("itens", exception.ParamName);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public void ValidarItens_WhenLoteHasSupportedQuantity_DoesNotThrow(int quantidade)
    {
        // Arrange
        var itens = CriarItens(quantidade);

        // Act
        var exception = Record.Exception(() => ManifestacaoDestinatarioServico.ValidarItens(itens));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ValidarItens_WhenLoteHasTwentyOneItems_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var itens = CriarItens(21);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => ManifestacaoDestinatarioServico.ValidarItens(itens));

        // Assert
        Assert.Equal("itens", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("curta")]
    public void ValidarItens_WhenJustificativaIsInvalidForOperacaoNaoRealizada_ThrowsArgumentException(string? justificativa)
    {
        // Arrange
        var item = new ManifestacaoDestinatarioItem(CriarChave(1), eTipoEventoNFe.OperacaoNaoRealizada, justificativa);

        // Act
        var exception = Assert.Throws<ArgumentException>(() => ManifestacaoDestinatarioServico.ValidarItens(new[] { item }));

        // Assert
        Assert.NotEmpty(exception.Message);
    }

    [Fact]
    public void ValidarItens_WhenJustificativaIsValidOnlyForOperacaoNaoRealizada_DoesNotThrow()
    {
        // Arrange
        var item = new ManifestacaoDestinatarioItem(CriarChave(1), eTipoEventoNFe.OperacaoNaoRealizada, "Justificativa válida com mais de quinze caracteres.");

        // Act
        var exception = Record.Exception(() => ManifestacaoDestinatarioServico.ValidarItens(new[] { item }));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CriarResultados_WhenRetornoIsPartial_MapsEachItemByChaveAndKeepsReconciliationPending()
    {
        // Arrange
        var primeiraChave = CriarChave(1);
        var segundaChave = CriarChave(2);
        var itens = new[]
        {
            new ManifestacaoDestinatarioItem(primeiraChave, eTipoEventoNFe.CienciaOperacao),
            new ManifestacaoDestinatarioItem(segundaChave, eTipoEventoNFe.ConfirmacaoOperacao)
        };
        var retorno = new retEnvEvento
        {
            cStat = 128,
            xMotivo = "Lote processado",
            retEvento = new List<retEvento>
            {
                CriarRetorno(segundaChave.Chave, eTipoEventoNFe.ConfirmacaoOperacao, 573, "Duplicidade de evento", "135000000000001")
            }
        };

        // Act
        var resultados = ManifestacaoDestinatarioServico.CriarResultados(itens, retorno);

        // Assert
        Assert.Equal(2, resultados.Count);
        Assert.Equal(primeiraChave.Chave, resultados[0].ChaveAcesso);
        Assert.Equal(eTipoEventoNFe.CienciaOperacao, resultados[0].TipoEvento);
        Assert.Equal(128, resultados[0].CodigoStatus);
        Assert.Equal(SituacaoManifestacaoDestinatario.Rejeitada, resultados[0].Situacao);
        Assert.Equal(segundaChave.Chave, resultados[1].ChaveAcesso);
        Assert.Equal(eTipoEventoNFe.ConfirmacaoOperacao, resultados[1].TipoEvento);
        Assert.Equal(573, resultados[1].CodigoStatus);
        Assert.Equal("135000000000001", resultados[1].Protocolo);
        Assert.Equal(SituacaoManifestacaoDestinatario.ReconciliacaoPendente, resultados[1].Situacao);
    }

    [Fact]
    public void CriarResultados_WhenSameChaveHasDifferentEventTypes_MapsEachReturnByType()
    {
        // Arrange
        var chave = CriarChave(1);
        var itens = new[]
        {
            new ManifestacaoDestinatarioItem(chave, eTipoEventoNFe.CienciaOperacao),
            new ManifestacaoDestinatarioItem(chave, eTipoEventoNFe.ConfirmacaoOperacao)
        };
        var retorno = new retEnvEvento
        {
            cStat = 128,
            xMotivo = "Lote processado",
            retEvento = new List<retEvento>
            {
                CriarRetorno(chave.Chave, eTipoEventoNFe.CienciaOperacao, 135, "Evento registrado", "135000000000001"),
                CriarRetorno(chave.Chave, eTipoEventoNFe.ConfirmacaoOperacao, 573, "Duplicidade de evento", "135000000000002")
            }
        };

        // Act
        var resultados = ManifestacaoDestinatarioServico.CriarResultados(itens, retorno);

        // Assert
        Assert.Equal(2, resultados.Count);
        Assert.Equal(eTipoEventoNFe.CienciaOperacao, resultados[0].TipoEvento);
        Assert.Equal(135, resultados[0].CodigoStatus);
        Assert.Equal("135000000000001", resultados[0].Protocolo);
        Assert.Equal(SituacaoManifestacaoDestinatario.Confirmada, resultados[0].Situacao);
        Assert.Equal(eTipoEventoNFe.ConfirmacaoOperacao, resultados[1].TipoEvento);
        Assert.Equal(573, resultados[1].CodigoStatus);
        Assert.Equal("135000000000002", resultados[1].Protocolo);
        Assert.Equal(SituacaoManifestacaoDestinatario.ReconciliacaoPendente, resultados[1].Situacao);
    }

    [Fact]
    public void Constructor_WhenJustificativaHasOnlyWhitespace_NormalizesToNull()
    {
        // Arrange
        var chave = CriarChave(1);

        // Act
        var item = new ManifestacaoDestinatarioItem(chave, eTipoEventoNFe.CienciaOperacao, "   ");

        // Assert
        Assert.Null(item.Justificativa);
    }

    private static List<ManifestacaoDestinatarioItem> CriarItens(int quantidade)
    {
        var itens = new List<ManifestacaoDestinatarioItem>(quantidade);
        for (var indice = 1; indice <= quantidade; indice++)
            itens.Add(new ManifestacaoDestinatarioItem(CriarChave(indice), eTipoEventoNFe.CienciaOperacao));

        return itens;
    }

    private static ChaveFiscal CriarChave(int numero) => new(eUF.SP, new DateTime(2026, 8, 5), "12345678000195", eModeloDocumento.NFe, 1, numero, eTipoEmissao.Normal, new Cnf("12345678"));

    private static retEvento CriarRetorno(
        string chave,
        eTipoEventoNFe tipoEvento,
        int codigoStatus,
        string motivo,
        string protocolo) => new()
    {
        infEvento = new infEventoRet
        {
            chNFe = chave,
            tpEvento = tipoEvento,
            nSeqEvento = 1,
            cStat = codigoStatus,
            xMotivo = motivo,
            nProt = protocolo,
            dhRegEvento = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc)
        }
    };
}
