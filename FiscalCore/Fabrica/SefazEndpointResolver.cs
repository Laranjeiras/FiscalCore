using FiscalCore.Tipos;
using FiscalCore.ValueObjects;

namespace FiscalCore.Fabrica;

public static class SefazEndpointResolver
{
    public static bool EhEventoAdquirente(eTipoEventoNFe tipoEvento)
    {
        return tipoEvento is eTipoEventoNFe.AceiteDebitoNFeCredito
            or eTipoEventoNFe.ImobilizacaoItemNFe;
    }

    public static eUF ObterUfDestino(eTipoEventoNFe tipoEvento, eUF ufEmitente)
    {
        if (EhEventoAdquirente(tipoEvento)
            || tipoEvento is eTipoEventoNFe.ConfirmacaoOperacao
                or eTipoEventoNFe.CienciaOperacao
                or eTipoEventoNFe.DesconhecimentoOperacao
                or eTipoEventoNFe.OperacaoNaoRealizada)
        {
            return eUF.AN;
        }

        return ufEmitente;
    }

    public static UrlSefaz ObterUrlEvento(
        eTipoEventoNFe tipoEvento,
        eTipoAmbiente tipoAmbiente,
        eModeloDocumento modelo,
        eUF ufEmitente)
    {
        var ufDestino = ObterUfDestino(tipoEvento, ufEmitente);
        if (ufDestino == eUF.AN)
        {
            return FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, tipoAmbiente, modelo, eUF.AN);
        }

        return FabricarUrl.ObterUrl(eTipoServico.CancelarNFe, tipoAmbiente, modelo, ufDestino);
    }
}
