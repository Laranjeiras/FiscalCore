using FiscalCore.Modelos.Retornos;
using System;
using System.Collections.Generic;

namespace FiscalCore.Servicos.DistribuicaoDFe;

/// <summary>Retorno de uma única transmissão de lote de manifestação.</summary>
public sealed class ManifestacaoDestinatarioLoteResultado
{
    public ManifestacaoDestinatarioLoteResultado(retEnvEvento retornoSefaz, IReadOnlyList<ManifestacaoDestinatarioResultado> resultados)
        : this(0, retornoSefaz, resultados, string.Empty, string.Empty)
    {
    }

    public ManifestacaoDestinatarioLoteResultado(
        int idLote,
        retEnvEvento retornoSefaz,
        IReadOnlyList<ManifestacaoDestinatarioResultado> resultados,
        string xmlEnvio,
        string xmlRetorno)
    {
        IdLote = idLote;
        RetornoSefaz = retornoSefaz ?? throw new ArgumentNullException(nameof(retornoSefaz));
        Resultados = resultados ?? throw new ArgumentNullException(nameof(resultados));
        XmlEnvio = xmlEnvio ?? throw new ArgumentNullException(nameof(xmlEnvio));
        XmlRetorno = xmlRetorno ?? throw new ArgumentNullException(nameof(xmlRetorno));
    }

    public int IdLote { get; }

    /// <summary>Retorno XML desserializado, preservado para consumidores que precisam dos campos originais.</summary>
    public retEnvEvento RetornoSefaz { get; }

    public IReadOnlyList<ManifestacaoDestinatarioResultado> Resultados { get; }
    public string XmlEnvio { get; }
    public string XmlRetorno { get; }
}
