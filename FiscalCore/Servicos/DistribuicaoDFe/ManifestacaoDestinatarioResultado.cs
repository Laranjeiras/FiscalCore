using System;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Resposta individual da SEFAZ correlacionada pela chave de acesso.</summary>
public sealed class ManifestacaoDestinatarioResultado
{
    public ManifestacaoDestinatarioResultado(
        string chaveAcesso,
        eTipoEventoNFe tipoEvento,
        int codigoStatus,
        string? motivo,
        string? protocolo,
        DateTime? dataRegistro,
        SituacaoManifestacaoDestinatario situacao)
        : this(chaveAcesso, tipoEvento, 1, codigoStatus, motivo, protocolo, dataRegistro, situacao)
    {
    }

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
        PossuiCodigoStatus = codigoStatus.HasValue;
        CodigoStatus = codigoStatus.GetValueOrDefault();
        Motivo = motivo;
        Protocolo = protocolo;
        DataRegistro = dataRegistro;
        Situacao = situacao;
    }

    public string ChaveAcesso { get; }
    public eTipoEventoNFe TipoEvento { get; }
    public int SequenciaEvento { get; }
    /// <summary>Compatível com o contrato 2.0.0; consulte PossuiCodigoStatus antes de usar.</summary>
    public int CodigoStatus { get; }
    public bool PossuiCodigoStatus { get; }
    public string? Motivo { get; }
    public string? Protocolo { get; }
    public DateTime? DataRegistro { get; }
    public SituacaoManifestacaoDestinatario Situacao { get; }
}
