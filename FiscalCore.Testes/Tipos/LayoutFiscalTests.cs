using FiscalCore.Tipos;
using Xunit;

namespace FiscalCore.Testes.Tipos;

/// <summary>
/// Os valores abaixo são fixados pelo MOC. Estes testes existem para que uma
/// alteração acidental precise ser deliberada.
/// </summary>
public sealed class LayoutFiscalTests
{
    [Fact]
    public void Comprimentos_DevemSeguirOMoc()
    {
        Assert.Equal(44, LayoutFiscal.TamanhoChaveAcesso);
        Assert.Equal(15, LayoutFiscal.TamanhoProtocolo);
        Assert.Equal(15, LayoutFiscal.TamanhoNsu);
        Assert.Equal(14, LayoutFiscal.TamanhoCnpj);
        Assert.Equal(11, LayoutFiscal.TamanhoCpf);
        Assert.Equal(8, LayoutFiscal.TamanhoCnf);
        Assert.Equal(20, LayoutFiscal.MaximoEventosPorLote);
    }

    [Fact]
    public void LimitesDeTexto_DevemSeguirOMoc()
    {
        Assert.Equal(15, LayoutFiscal.JustificativaMinima);
        Assert.Equal(255, LayoutFiscal.JustificativaMaxima);
        Assert.Equal(15, LayoutFiscal.CorrecaoMinima);
        Assert.Equal(1000, LayoutFiscal.CorrecaoMaxima);
    }

    /// <summary>
    /// Cada campo começa onde o anterior termina e o conjunto cobre exatamente os
    /// 44 caracteres da chave — sem lacuna nem sobreposição.
    /// </summary>
    [Fact]
    public void PosicoesDaChave_DevemSerContiguasECobrirAChaveInteira()
    {
        var campos = new[]
        {
            (LayoutFiscal.PosicaoChave.Uf, LayoutFiscal.PosicaoChave.UfTamanho),
            (LayoutFiscal.PosicaoChave.AnoMesEmissao, LayoutFiscal.PosicaoChave.AnoMesEmissaoTamanho),
            (LayoutFiscal.PosicaoChave.Cnpj, LayoutFiscal.PosicaoChave.CnpjTamanho),
            (LayoutFiscal.PosicaoChave.Modelo, LayoutFiscal.PosicaoChave.ModeloTamanho),
            (LayoutFiscal.PosicaoChave.Serie, LayoutFiscal.PosicaoChave.SerieTamanho),
            (LayoutFiscal.PosicaoChave.Numero, LayoutFiscal.PosicaoChave.NumeroTamanho),
            (LayoutFiscal.PosicaoChave.TipoEmissao, LayoutFiscal.PosicaoChave.TipoEmissaoTamanho),
            (LayoutFiscal.PosicaoChave.Cnf, LayoutFiscal.PosicaoChave.CnfTamanho),
            (LayoutFiscal.PosicaoChave.DigitoVerificador, LayoutFiscal.PosicaoChave.DigitoVerificadorTamanho),
        };

        var proximoOffsetEsperado = 0;
        foreach (var (offset, tamanho) in campos)
        {
            Assert.Equal(proximoOffsetEsperado, offset);
            proximoOffsetEsperado = offset + tamanho;
        }

        Assert.Equal(LayoutFiscal.TamanhoChaveAcesso, proximoOffsetEsperado);
    }

    [Fact]
    public void PosicaoDoCnpjNaChave_DeveTerOTamanhoDeUmCnpj()
        => Assert.Equal(LayoutFiscal.TamanhoCnpj, LayoutFiscal.PosicaoChave.CnpjTamanho);

    [Fact]
    public void PosicaoDoCnfNaChave_DeveTerOTamanhoDeUmCnf()
        => Assert.Equal(LayoutFiscal.TamanhoCnf, LayoutFiscal.PosicaoChave.CnfTamanho);
}
