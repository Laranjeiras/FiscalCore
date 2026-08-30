using FiscalCore.Modelos.Retornos;
using FiscalCore.Servicos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FiscalCore.Testes;

public sealed class ManifestacaoDestinatarioServicoTests
{
    [Fact]
    public void ValidarItens_ComLoteVazio_DeveLancarArgumentOutOfRange()
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
    public void ValidarItens_ComQuantidadeSuportada_NaoDeveLancar(int quantidade)
    {
        // Arrange
        var itens = CriarItens(quantidade);

        // Act
        var exception = Record.Exception(() => ManifestacaoDestinatarioServico.ValidarItens(itens));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ValidarItens_ComVinteEUmItens_DeveLancarArgumentOutOfRange()
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
    public void ValidarItens_ComJustificativaInvalidaParaOperacaoNaoRealizada_DeveLancarArgumentException(string? justificativa)
    {
        // Arrange
        var item = new ManifestacaoDestinatarioItem(CriarChave(1), eTipoEventoNFe.OperacaoNaoRealizada, justificativa);

        // Act
        var exception = Assert.Throws<ArgumentException>(() => ManifestacaoDestinatarioServico.ValidarItens(new[] { item }));

        // Assert
        Assert.NotEmpty(exception.Message);
    }

    [Fact]
    public void ValidarItens_ComJustificativaValidaApenasParaOperacaoNaoRealizada_NaoDeveLancar()
    {
        // Arrange
        var item = new ManifestacaoDestinatarioItem(CriarChave(1), eTipoEventoNFe.OperacaoNaoRealizada, "Justificativa válida com mais de quinze caracteres.");

        // Act
        var exception = Record.Exception(() => ManifestacaoDestinatarioServico.ValidarItens(new[] { item }));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CriarResultados_ComRetornoParcial_DeveMapearCadaItemPorChaveEManterConciliacaoPendente()
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
        Assert.False(resultados[0].PossuiCodigoStatus);
        Assert.Equal(0, resultados[0].CodigoStatus);
        Assert.Equal(SituacaoManifestacaoDestinatario.ReconciliacaoPendente, resultados[0].Situacao);
        Assert.Equal(segundaChave.Chave, resultados[1].ChaveAcesso);
        Assert.Equal(eTipoEventoNFe.ConfirmacaoOperacao, resultados[1].TipoEvento);
        Assert.Equal(573, resultados[1].CodigoStatus);
        Assert.Equal("135000000000001", resultados[1].Protocolo);
        Assert.Equal(SituacaoManifestacaoDestinatario.ReconciliacaoPendente, resultados[1].Situacao);
    }

    [Fact]
    public void CriarResultados_ComMesmaChaveETiposDeEventoDistintos_DeveMapearCadaRetornoPorTipo()
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
    public void Construtor_ComJustificativaApenasEmBranco_DeveNormalizarParaNulo()
    {
        // Arrange
        var chave = CriarChave(1);

        // Act
        var item = new ManifestacaoDestinatarioItem(chave, eTipoEventoNFe.CienciaOperacao, "   ");

        // Assert
        Assert.Null(item.Justificativa);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void ValidarItens_ComSequenciaInvalida_DeveLancarArgumentOutOfRange(int sequencia)
    {
        // Arrange
        var item = new ManifestacaoDestinatarioItem(CriarChave(1), eTipoEventoNFe.ConfirmacaoOperacao, sequencia);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => ManifestacaoDestinatarioServico.ValidarItens(new[] { item }));

        // Assert
        Assert.Equal("itens", exception.ParamName);
    }

    [Fact]
    public void CriarResultados_ComEvento136_DeveRetornarRegistradaSemVinculo()
    {
        // Arrange
        var chave = CriarChave(1);
        var item = new ManifestacaoDestinatarioItem(chave, eTipoEventoNFe.ConfirmacaoOperacao, 2);
        var retorno = new retEnvEvento
        {
            cStat = 128,
            retEvento = new List<retEvento> { CriarRetorno(chave.Chave, eTipoEventoNFe.ConfirmacaoOperacao, 136, "Registrado sem vínculo", "135000000000001", 2) }
        };

        // Act
        var resultado = ManifestacaoDestinatarioServico.CriarResultados(new[] { item }, retorno).Single();

        // Assert
        Assert.Equal(2, resultado.SequenciaEvento);
        Assert.Equal(136, resultado.CodigoStatus);
        Assert.Equal(SituacaoManifestacaoDestinatario.RegistradaSemVinculo, resultado.Situacao);
    }

    [Theory]
    [InlineData(WebExceptionStatus.NameResolutionFailure, true)]
    [InlineData(WebExceptionStatus.ConnectFailure, true)]
    [InlineData(WebExceptionStatus.ProxyNameResolutionFailure, true)]
    [InlineData(WebExceptionStatus.TrustFailure, true)]
    [InlineData(WebExceptionStatus.SecureChannelFailure, true)]
    [InlineData(WebExceptionStatus.Timeout, false)]
    [InlineData(WebExceptionStatus.SendFailure, false)]
    [InlineData(WebExceptionStatus.ReceiveFailure, false)]
    [InlineData(WebExceptionStatus.ConnectionClosed, false)]
    public void EhFalhaSeguraAntesDoEnvio_ComStatusClassificado_DeveRetornarOEsperado(WebExceptionStatus status, bool esperado)
    {
        // Arrange
        var exception = new WebException("falha", status);

        // Act
        var resultado = ManifestacaoDestinatarioServico.EhFalhaSeguraAntesDoEnvio(exception);

        // Assert
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void EhFalhaSeguraAntesDoEnvio_ComCancelamentoAmbiguo_DeveRetornarFalso()
    {
        // Act
        var resultado = ManifestacaoDestinatarioServico.EhFalhaSeguraAntesDoEnvio(new TaskCanceledException());

        // Assert
        Assert.False(resultado);
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
        string protocolo,
        int sequencia = 1) => new()
    {
        infEvento = new infEventoRet
        {
            chNFe = chave,
            tpEvento = tipoEvento,
            nSeqEvento = sequencia,
            cStat = codigoStatus,
            xMotivo = motivo,
            nProt = protocolo,
            dhRegEvento = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc)
        }
    };
}
