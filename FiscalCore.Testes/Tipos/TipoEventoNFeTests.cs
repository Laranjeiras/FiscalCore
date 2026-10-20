using FiscalCore.Tipos;
using Xunit;

namespace FiscalCore.Testes.Tipos;

public class TipoEventoNFeTests
{
    [Fact]
    public void ValidaQueValoresLegadosNaoForamRenumerados()
    {
        // EVT-05: Nao renumerar valores anteriores do enum
        Assert.Equal(0, (int)eTipoEventoNFe.NenhumEventoInformado);
        Assert.Equal(110110, (int)eTipoEventoNFe.NFeCartaCorrecao);
        Assert.Equal(110111, (int)eTipoEventoNFe.NFeCancelamento);
        Assert.Equal(110112, (int)eTipoEventoNFe.NFeCancelamentoSubstituicao);
        Assert.Equal(110130, (int)eTipoEventoNFe.ComprovanteEntregaNFe);
        Assert.Equal(110131, (int)eTipoEventoNFe.CancelamentoComprovanteEntregaNFe);
        Assert.Equal(110140, (int)eTipoEventoNFe.NFceEpec);
        Assert.Equal(210200, (int)eTipoEventoNFe.ConfirmacaoOperacao);
        Assert.Equal(210210, (int)eTipoEventoNFe.CienciaOperacao);
        Assert.Equal(210220, (int)eTipoEventoNFe.DesconhecimentoOperacao);
        Assert.Equal(210240, (int)eTipoEventoNFe.OperacaoNaoRealizada);
        Assert.Equal(610552, (int)eTipoEventoNFe.RegistroPassagemAutomaticoMDFe);
        Assert.Equal(610610, (int)eTipoEventoNFe.EventosMDFeAutorizado);
        Assert.Equal(610500, (int)eTipoEventoNFe.EventosRegistroPassagemAutorizacao);
        Assert.Equal(610501, (int)eTipoEventoNFe.EventosRegistroCancelamentoPassagem);
        Assert.Equal(610510, (int)eTipoEventoNFe.EventosRegistroPassagemNFeMDFe);
        Assert.Equal(610514, (int)eTipoEventoNFe.EventosRegistroPassagemNFeCTe);
        Assert.Equal(610550, (int)eTipoEventoNFe.EventosRegistroPassagemAutomatica);
        Assert.Equal(610554, (int)eTipoEventoNFe.EventosRegistroPassagemAutomaticaMDFeCTe);
        Assert.Equal(610600, (int)eTipoEventoNFe.EventosRegistroAutorizaçãoCTeNFe);
        Assert.Equal(610601, (int)eTipoEventoNFe.EventosRegistroCancelamentoCTeNFe);
        Assert.Equal(610611, (int)eTipoEventoNFe.EventosMDFeCancelado);
        Assert.Equal(610614, (int)eTipoEventoNFe.EventosMDFeAutorizadoCTe);
        Assert.Equal(610615, (int)eTipoEventoNFe.EventosCancelamentoMDFeCTe);
    }

    [Fact]
    public void ValidaQueEventosRtcEstaoCorretos()
    {
        // EVT-01 e EVT-05
        Assert.Equal(211128, (int)eTipoEventoNFe.AceiteDebitoNFeCredito);

        // EVT-02 e EVT-05
        Assert.Equal(211130, (int)eTipoEventoNFe.ImobilizacaoItemNFe);
    }
}
