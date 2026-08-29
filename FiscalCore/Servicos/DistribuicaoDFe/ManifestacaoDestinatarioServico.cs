using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe;

public class ManifestacaoDestinatarioServico : BaseSefazServico<ManifestacaoDestinatarioServico>
{
    private const int QuantidadeMinimaLote = 1;
    private const int QuantidadeMaximaLote = LayoutFiscal.MaximoEventosPorLote;
    private const int StatusEventoRegistrado = 135;
    private const int StatusEventoRegistradoSemVinculo = 136;
    private const int StatusDuplicidadeEvento = 573;
    private static int proximoIdLote = (int)(DateTime.UtcNow.Ticks % 1_000_000_000);

    public ManifestacaoDestinatarioServico(
        ConfiguracaoServico config,
        IStorageContext storageContext,
        ITransmitirSefazCommand transmitir,
        ILogger<ManifestacaoDestinatarioServico> logger)
        : base(config, transmitir, logger, storageContext)
    {
    }

    /// <summary>
    /// Mantém o contrato público legado. A transmissão usa o mesmo pipeline de um lote unitário.
    /// </summary>
    public async Task<retEnvEvento> ManifestarAsync(
        ChaveFiscal chaveNFe,
        eTipoEventoNFe tipoEvento,
        string justificativa,
        CancellationToken cancellation)
    {
        var item = new ManifestacaoDestinatarioItem(chaveNFe, tipoEvento, sequenciaEvento: 1, justificativa);
        var resultado = await ManifestarLoteAsync(new[] { item }, cancellation).ConfigureAwait(false);
        return resultado.RetornoSefaz;
    }

    /// <summary>
    /// Transmite de 1 a 20 manifestações em um único envEvento. Cada infEvento é assinada individualmente.
    /// O status 573 é devolvido como reconciliação pendente para que o consumidor confirme a evidência local.
    /// </summary>
    public async Task<ManifestacaoDestinatarioLoteResultado> ManifestarLoteAsync(
        IReadOnlyList<ManifestacaoDestinatarioItem> itens,
        CancellationToken cancellation = default) =>
        await ManifestarLoteAsync(itens, cancellation, null).ConfigureAwait(false);

    public async Task<ManifestacaoDestinatarioLoteResultado> ManifestarLoteAsync(
        IReadOnlyList<ManifestacaoDestinatarioItem> itens,
        CancellationToken cancellation,
        Func<string, int, CancellationToken, Task>? antesDeTransmitir)
    {
        ValidarItens(itens);
        cancellation.ThrowIfCancellationRequested();
        logger.LogInformation("INICIANDO MANIFESTACAO DO DESTINATARIO EM LOTE COM {Quantidade} EVENTOS", itens.Count);

        var idLote = GerarIdLote();
        var envio = CriarEnvEvento(itens, idLote);
        var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(envio);
        var arquivoEnvio = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve.xml", configuracao));
        await SalvarLog(arquivoEnvio, xmlEvento, cancellation).ConfigureAwait(false);

        if (configuracao.ValidarXmlSchema)
        {
            ValidarXml(eTipoServico.ManifestacaoDestinatario, configuracao, xmlEvento);
        }

        var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
        var sefazUrl = FabricarUrl.ObterUrl(
            eTipoServico.ManifestacaoDestinatario,
            configuracao.TipoAmbiente,
            eModeloDocumento.NFe,
            eUF.AN);
        if (antesDeTransmitir is not null)
            await antesDeTransmitir(xmlEvento, idLote, cancellation).ConfigureAwait(false);

        var cronometro = Stopwatch.StartNew();
        var xmlRetorno = await TransmitirComRetryAntesDoEnvioAsync(transmitir, logger, sefazUrl, envelope!, cancellation).ConfigureAwait(false);
        cronometro.Stop();
        var xmlRetornoLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;

        var arquivoRetorno = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve.xml", configuracao));
        await SalvarLog(arquivoRetorno, xmlRetornoLimpo, cancellation).ConfigureAwait(false);

        var retornoSefaz = XmlUtils.XmlStringParaClasse<retEnvEvento>(xmlRetornoLimpo);
        var resultados = CriarResultados(itens, retornoSefaz);
        logger.LogInformation("Manifestação destinatário: lote {IdLote}, duração {DuracaoMs}ms, cStat lote {CodigoStatus}", idLote, cronometro.ElapsedMilliseconds, retornoSefaz.cStat);
        return new ManifestacaoDestinatarioLoteResultado(idLote, retornoSefaz, resultados, xmlEvento, xmlRetornoLimpo);
    }

    internal envEvento CriarEnvEvento(IReadOnlyList<ManifestacaoDestinatarioItem> itens, int idLote)
    {
        var eventos = new List<evento>(itens.Count);
        foreach (var item in itens)
        {
            eventos.Add(CriarEvento(item));
        }

        return envEvento.Criar(configuracao.VersaoManifestacaoDestinatario.Descricao(), idLote, eventos);
    }

    internal static void ValidarItens(IReadOnlyList<ManifestacaoDestinatarioItem> itens)
    {
        if (itens == null)
        {
            throw new ArgumentNullException(nameof(itens));
        }

        if (itens.Count < QuantidadeMinimaLote || itens.Count > QuantidadeMaximaLote)
        {
            throw new ArgumentOutOfRangeException(nameof(itens), itens.Count, "O lote deve conter entre 1 e 20 manifestações.");
        }

        foreach (var item in itens)
        {
            if (item == null)
            {
                throw new ArgumentException("O lote não pode conter itens nulos.", nameof(itens));
            }

            if (!PodeManifestar(item.TipoEvento))
            {
                throw new ArgumentException("Evento não permitido nesse serviço.", nameof(itens));
            }

            if (item.SequenciaEvento is < 1 or > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(itens), "A sequência do evento deve ser 1 ou 2.");
            }

            if (item.TipoEvento == eTipoEventoNFe.CienciaOperacao && item.SequenciaEvento != 1)
            {
                throw new ArgumentException("Ciência da Operação aceita somente sequência 1.", nameof(itens));
            }

            ValidarJustificativa(item);
        }
    }

    private evento CriarEvento(ManifestacaoDestinatarioItem item)
    {
        var versao = configuracao.VersaoManifestacaoDestinatario.Descricao();
        var infEvento = new infEventoEnv
        {
            chNFe = item.ChaveNFe.Chave,
            CNPJ = configuracao.Emitente!.CNPJ,
            CPF = configuracao.Emitente.CPF,
            cOrgao = eUF.AN,
            dhEvento = DateTime.Now,
            nSeqEvento = item.SequenciaEvento,
            tpAmb = configuracao.TipoAmbiente,
            tpEvento = item.TipoEvento,
            verEvento = versao,
            Id = "ID" + ((int)item.TipoEvento) + item.ChaveNFe.Chave + item.SequenciaEvento.ToString().PadLeft(LayoutFiscal.TamanhoSequenciaEvento, '0'),
            detEvento = new detEvento
            {
                versao = versao,
                descEvento = (item.TipoEvento.Descricao() ?? string.Empty).RemoverAcentos(),
                xJust = item.Justificativa
            }
        };

        var evento = Modelos.Eventos.evento.CriarEvento(versao, infEvento);
        evento.Assinar(
            configuracao.ConfigCertificado.Certificado,
            configuracao.ConfigCertificado.SignatureMethodSignedXml,
            configuracao.ConfigCertificado.DigestMethodReference);
        return evento;
    }

    internal static IReadOnlyList<ManifestacaoDestinatarioResultado> CriarResultados(
        IReadOnlyList<ManifestacaoDestinatarioItem> itens,
        retEnvEvento retornoSefaz)
    {
        ArgumentNullException.ThrowIfNull(retornoSefaz);
        var retornosPorEvento = (retornoSefaz.retEvento ?? new List<Modelos.Retornos.retEvento>())
            .Where(retorno => retorno?.infEvento?.chNFe != null
                && retorno.infEvento.tpEvento.HasValue
                && retorno.infEvento.nSeqEvento.HasValue)
            .GroupBy(retorno => CriarChaveCorrelacao(
                retorno.infEvento.chNFe,
                retorno.infEvento.tpEvento.GetValueOrDefault(),
                retorno.infEvento.nSeqEvento.GetValueOrDefault()))
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Last().infEvento);

        return itens.Select(item =>
        {
            if (!retornosPorEvento.TryGetValue(CriarChaveCorrelacao(item.ChaveNFe.Chave, item.TipoEvento, item.SequenciaEvento), out var retorno))
            {
                return new ManifestacaoDestinatarioResultado(
                    item.ChaveNFe.Chave,
                    item.TipoEvento,
                    item.SequenciaEvento,
                    null,
                    retornoSefaz.xMotivo,
                    null,
                    null,
                    SituacaoManifestacaoDestinatario.ReconciliacaoPendente);
            }

            return new ManifestacaoDestinatarioResultado(
                item.ChaveNFe.Chave,
                item.TipoEvento,
                item.SequenciaEvento,
                retorno.cStat,
                retorno.xMotivo,
                retorno.nProt,
                retorno.dhRegEvento == DateTime.MinValue ? null : retorno.dhRegEvento,
                ObterSituacao(retorno.cStat));
        }).ToList();
    }

    private static SituacaoManifestacaoDestinatario ObterSituacao(int codigoStatus)
    {
        return codigoStatus switch
        {
            StatusEventoRegistrado => SituacaoManifestacaoDestinatario.Confirmada,
            StatusEventoRegistradoSemVinculo => SituacaoManifestacaoDestinatario.RegistradaSemVinculo,
            StatusDuplicidadeEvento => SituacaoManifestacaoDestinatario.ReconciliacaoPendente,
            _ => SituacaoManifestacaoDestinatario.Rejeitada
        };
    }

    private static void ValidarJustificativa(ManifestacaoDestinatarioItem item)
    {
        if (item.TipoEvento == eTipoEventoNFe.OperacaoNaoRealizada)
        {
            if (string.IsNullOrWhiteSpace(item.Justificativa)
                || item.Justificativa.Length < LayoutFiscal.JustificativaMinima
                || item.Justificativa.Length > LayoutFiscal.JustificativaMaxima)
            {
                throw new ArgumentException("A justificativa de Operação não Realizada deve conter entre 15 e 255 caracteres.", nameof(item));
            }

            return;
        }

        if (!string.IsNullOrWhiteSpace(item.Justificativa))
        {
            throw new ArgumentException("Justificativa é permitida somente para Operação não Realizada.", nameof(item));
        }
    }

    private static bool PodeManifestar(eTipoEventoNFe tipoEvento)
    {
        return tipoEvento == eTipoEventoNFe.CienciaOperacao
            || tipoEvento == eTipoEventoNFe.ConfirmacaoOperacao
            || tipoEvento == eTipoEventoNFe.DesconhecimentoOperacao
            || tipoEvento == eTipoEventoNFe.OperacaoNaoRealizada;
    }

    internal static async Task<string> TransmitirComRetryAntesDoEnvioAsync(
        ITransmitirSefazCommand transmitir,
        ILogger logger,
        UrlSefaz sefazUrl,
        System.Xml.XmlDocument envelope,
        CancellationToken cancellation)
    {
        const int maximoTentativas = 3;
        for (var tentativa = 1; tentativa <= maximoTentativas; tentativa++)
        {
            try
            {
                return await transmitir.TransmitirAsync(sefazUrl, envelope, cancellation).ConfigureAwait(false);
            }
            catch (Exception ex) when (EhFalhaSeguraAntesDoEnvio(ex))
            {
                if (tentativa == maximoTentativas)
                    throw new TransmissaoNaoIniciadaException("A transmissão não foi iniciada após falha de conexão pré-envio.", ex);

                var atraso = TimeSpan.FromMilliseconds((100 * (1 << (tentativa - 1))) + Random.Shared.Next(0, 100));
                logger.LogWarning(ex, "Falha pré-envio na manifestação. Nova tentativa {Tentativa} em {AtrasoMs}ms.", tentativa + 1, atraso.TotalMilliseconds);
                await Task.Delay(atraso, cancellation).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException("Fluxo de repetição de transmissão inválido.");
    }

    public static bool EhFalhaSeguraAntesDoEnvio(Exception exception) =>
        (exception is WebException webException && webException.Status is WebExceptionStatus.NameResolutionFailure
            or WebExceptionStatus.ConnectFailure
            or WebExceptionStatus.ProxyNameResolutionFailure
            or WebExceptionStatus.TrustFailure
            or WebExceptionStatus.SecureChannelFailure)
        || (exception is HttpRequestException httpException && httpException.HttpRequestError is HttpRequestError.NameResolutionError
            or HttpRequestError.ConnectionError
            or HttpRequestError.SecureConnectionError);

    private static int GerarIdLote() => Interlocked.Increment(ref proximoIdLote) & int.MaxValue;

    private static string CriarChaveCorrelacao(string chave, eTipoEventoNFe tipoEvento, int sequencia) =>
        $"{chave}|{(int)tipoEvento}|{sequencia}";
}
