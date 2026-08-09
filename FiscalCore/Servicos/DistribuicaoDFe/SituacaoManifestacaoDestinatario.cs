namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Situação semântica da resposta individual do evento na SEFAZ.</summary>
public enum SituacaoManifestacaoDestinatario
{
    Rejeitada = 0,
    Confirmada = 1,
    ReconciliacaoPendente = 2
}
