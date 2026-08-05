using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Dados de uma manifestação do destinatário a transmitir em lote.</summary>
public sealed class ManifestacaoDestinatarioItem
{
    public ManifestacaoDestinatarioItem(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, string? justificativa = null)
    {
        ChaveNFe = chaveNFe ?? throw new ArgumentNullException(nameof(chaveNFe));
        TipoEvento = tipoEvento;
        Justificativa = string.IsNullOrWhiteSpace(justificativa) ? null : justificativa.Trim();
    }

    public ChaveFiscal ChaveNFe { get; }

    public eTipoEventoNFe TipoEvento { get; }

    public string? Justificativa { get; }
}
