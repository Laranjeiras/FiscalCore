using System;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Resposta individual da SEFAZ correlacionada pela chave de acesso.</summary>
public sealed class ManifestacaoDestinatarioResultado
{
    public ManifestacaoDestinatarioResultado(
        string chaveAcesso,
        eTipoEventoNFe tipoEvento,
        int sequenciaEvento,
        int? codigoStatus,
        string? motivo,
        string? protocolo,
        DateTime? dataRegistro,
        SituacaoManifestacaoDestinatario situacao)
    {
        ChaveAcesso = chaveAcesso ?? throw new ArgumentNullException(nameof(chaveAcesso));
        TipoEvento = tipoEvento;
        SequenciaEvento = sequenciaEvento;
        CodigoStatus = codigoStatus;
        Motivo = motivo;
        Protocolo = protocolo;
        DataRegistro = dataRegistro;
        Situacao = situacao;
    }

    public string ChaveAcesso { get; }
    public eTipoEventoNFe TipoEvento { get; }
    public int SequenciaEvento { get; }
    public int? CodigoStatus { get; }
    public string? Motivo { get; }
    public string? Protocolo { get; }
    public DateTime? DataRegistro { get; }
    public SituacaoManifestacaoDestinatario Situacao { get; }
}
