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
        // Eventos tradicionais de manifestação do destinatário (210200, 210210, 210220, 210240) vão para o Ambiente Nacional (AN)
        if (tipoEvento is eTipoEventoNFe.ConfirmacaoOperacao
            or eTipoEventoNFe.CienciaOperacao
            or eTipoEventoNFe.DesconhecimentoOperacao
            or eTipoEventoNFe.OperacaoNaoRealizada)
        {
            return eUF.AN;
        }

        // Eventos RTC do adquirente (211128, 211130) são autorizados na SVRS (NT 2025.002 v1.51 §8.1)
        if (EhEventoAdquirente(tipoEvento))
        {
            return ufEmitente;
        }

        return ufEmitente;
    }

    public static UrlSefaz ObterUrlEvento(
        eTipoEventoNFe tipoEvento,
        eTipoAmbiente tipoAmbiente,
        eModeloDocumento modelo,
        eUF ufEmitente)
    {
        if (EhEventoAdquirente(tipoEvento))
        {
            return ObterUrlSvrsRecepcaoEvento(tipoAmbiente, modelo, ufEmitente);
        }

        var ufDestino = ObterUfDestino(tipoEvento, ufEmitente);
        if (ufDestino == eUF.AN)
        {
            return FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, tipoAmbiente, modelo, eUF.AN);
        }

        return FabricarUrl.ObterUrl(eTipoServico.CancelarNFe, tipoAmbiente, modelo, ufDestino);
    }

    public static UrlSefaz ObterUrlSvrsRecepcaoEvento(eTipoAmbiente tipoAmbiente, eModeloDocumento modelo, eUF ufEmitente)
    {
        var urlBase = tipoAmbiente == eTipoAmbiente.Producao
            ? "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"
            : "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx";

        var action = $"{urlBase}?op=nfeRecepcaoEvento";
        return new UrlSefaz(eTipoServico.CancelarNFe, ufEmitente, tipoAmbiente, modelo, urlBase, action);
    }
}
