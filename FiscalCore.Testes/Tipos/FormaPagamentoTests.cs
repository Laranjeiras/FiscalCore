using FiscalCore.NotaFiscal.Informacoes.Pagamento;
using FiscalCore.Tipos;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Xunit;

namespace FiscalCore.Testes.Tipos;

public sealed class FormaPagamentoTests
{
    private static readonly int[] CodigosOficiais =
        [1, 2, 3, 4, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 90, 91, 99];

    [Fact]
    public void ContratosPublicos_DevemCobrirMesmosCodigosOficiais()
    {
        var atuais = Enum.GetValues<eFormaPagamento>().Select(valor => (int)valor).Order().ToArray();
        var legados = Enum.GetValues<FormaPagamento>().Select(valor => (int)valor).Order().ToArray();

        Assert.Equal(CodigosOficiais, atuais);
        Assert.Equal(CodigosOficiais, legados);
    }

    [Theory]
    [InlineData(eFormaPagamento.CreditoLoja, "05", "Cartão da Loja (Private Label), Crediário Digital, Outros Crediários")]
    [InlineData(eFormaPagamento.PixPagamentoInstantaneo, "17", "Pagamento Instantâneo (PIX) - Dinâmico")]
    [InlineData(eFormaPagamento.TransferenciaBancariaCarteiraDigital, "18", "TED (Transferência Eletrônica Disponível)")]
    [InlineData(eFormaPagamento.PixPagamentoInstantaneoEstatico, "20", "Pagamento Instantâneo (PIX) - Estático")]
    [InlineData(eFormaPagamento.CreditoEmLoja, "21", "Crédito em Loja")]
    [InlineData(eFormaPagamento.PagamentoEletronicoNaoInformadoFalhaHardware, "22", "Pagamento Eletrônico não Informado - falha de hardware do sistema emissor")]
    [InlineData(eFormaPagamento.PixPagamentoInstantaneoAutomatico, "23", "Pagamento Instantâneo (PIX) - Automático")]
    [InlineData(eFormaPagamento.TefBookTransfer, "24", "TEF - \"Book Transfer\"")]
    [InlineData(eFormaPagamento.PagamentoPosterior, "91", "Pagamento Posterior")]
    public void FormaPagamento_AlteracaoOficial_DevePreservarXmlEDescricao(
        eFormaPagamento valor,
        string codigoXml,
        string descricao)
    {
        var membro = typeof(eFormaPagamento).GetMember(valor.ToString()).Single();

        Assert.Equal(codigoXml, membro.GetCustomAttribute<XmlEnumAttribute>()?.Name);
        Assert.Equal(descricao, membro.GetCustomAttribute<DescriptionAttribute>()?.Description);
    }
}
