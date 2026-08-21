using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Dados de uma manifestação do destinatário a transmitir em lote.</summary>
public sealed class ManifestacaoDestinatarioItem
{
    public ManifestacaoDestinatarioItem(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, string? justificativa = null)
        : this(chaveNFe, tipoEvento, sequenciaEvento: 1, justificativa)
    {
    }

    public ManifestacaoDestinatarioItem(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, int sequenciaEvento, string? justificativa = null)
    {
        ChaveNFe = chaveNFe ?? throw new ArgumentNullException(nameof(chaveNFe));
        TipoEvento = tipoEvento;
        SequenciaEvento = sequenciaEvento;
        Justificativa = string.IsNullOrWhiteSpace(justificativa) ? null : justificativa.Trim();
    }

    public ChaveFiscal ChaveNFe { get; }

    public eTipoEventoNFe TipoEvento { get; }

    public int SequenciaEvento { get; }

    public string? Justificativa { get; }
}
