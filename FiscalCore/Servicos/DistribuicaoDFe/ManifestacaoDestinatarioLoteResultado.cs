using FiscalCore.Modelos.Retornos;
using System;
using System.Collections.Generic;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Retorno de uma única transmissão de lote de manifestação.</summary>
public sealed class ManifestacaoDestinatarioLoteResultado
{
    public ManifestacaoDestinatarioLoteResultado(retEnvEvento retornoSefaz, IReadOnlyList<ManifestacaoDestinatarioResultado> resultados)
    {
        RetornoSefaz = retornoSefaz ?? throw new ArgumentNullException(nameof(retornoSefaz));
        Resultados = resultados ?? throw new ArgumentNullException(nameof(resultados));
    }

    /// <summary>Retorno XML desserializado, preservado para consumidores que precisam dos campos originais.</summary>
    public retEnvEvento RetornoSefaz { get; }

    public IReadOnlyList<ManifestacaoDestinatarioResultado> Resultados { get; }
}
