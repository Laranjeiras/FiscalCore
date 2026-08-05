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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe;

public class ManifestacaoDestinatarioServico : BaseSefazServico<ManifestacaoDestinatarioServico>
{
    private const int QuantidadeMinimaLote = 1;
    private const int QuantidadeMaximaLote = 20;
    private const int SequenciaEvento = 1;
    private const int StatusEventoRegistrado = 135;
    private const int StatusDuplicidadeEvento = 573;

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
        var item = new ManifestacaoDestinatarioItem(chaveNFe, tipoEvento, justificativa);
        var resultado = await ManifestarLoteAsync(new[] { item }, cancellation).ConfigureAwait(false);
        return resultado.RetornoSefaz;
    }

    /// <summary>
    /// Transmite de 1 a 20 manifestações em um único envEvento. Cada infEvento é assinada individualmente.
    /// O status 573 é devolvido como reconciliação pendente para que o consumidor confirme a evidência local.
    /// </summary>
    public async Task<ManifestacaoDestinatarioLoteResultado> ManifestarLoteAsync(
        IReadOnlyList<ManifestacaoDestinatarioItem> itens,
        CancellationToken cancellation = default)
    {
        ValidarItens(itens);
        logger.LogInformation("INICIANDO MANIFESTACAO DO DESTINATARIO EM LOTE COM {Quantidade} EVENTOS", itens.Count);

        var envio = CriarEnvEvento(itens);
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
        var xmlRetorno = await transmitir.TransmitirAsync(sefazUrl, envelope!).ConfigureAwait(false);
        var xmlRetornoLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;

        var arquivoRetorno = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve.xml", configuracao));
        await SalvarLog(arquivoRetorno, xmlRetornoLimpo, cancellation).ConfigureAwait(false);

        var retornoSefaz = XmlUtils.XmlStringParaClasse<retEnvEvento>(xmlRetornoLimpo);
        var resultados = CriarResultados(itens, retornoSefaz);
        return new ManifestacaoDestinatarioLoteResultado(retornoSefaz, resultados);
    }

    internal envEvento CriarEnvEvento(IReadOnlyList<ManifestacaoDestinatarioItem> itens)
    {
        var eventos = new List<evento>(itens.Count);
        foreach (var item in itens)
        {
            eventos.Add(CriarEvento(item));
        }

        return envEvento.Criar(configuracao.VersaoManifestacaoDestinatario.Descricao(), SequenciaEvento, eventos);
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
            nSeqEvento = SequenciaEvento,
            tpAmb = configuracao.TipoAmbiente,
            tpEvento = item.TipoEvento,
            verEvento = versao,
            Id = "ID" + ((int)item.TipoEvento) + item.ChaveNFe.Chave + SequenciaEvento.ToString().PadLeft(2, '0'),
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
        var retornosPorEvento = (retornoSefaz.retEvento ?? new List<Modelos.Retornos.retEvento>())
            .Where(retorno => retorno?.infEvento?.chNFe != null && retorno.infEvento.tpEvento.HasValue)
            .GroupBy(retorno => new
            {
                ChaveAcesso = retorno.infEvento.chNFe,
                TipoEvento = retorno.infEvento.tpEvento,
                Sequencia = retorno.infEvento.nSeqEvento
            })
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Last().infEvento);

        return itens.Select(item =>
        {
            var chaveRetorno = new
            {
                ChaveAcesso = item.ChaveNFe.Chave,
                TipoEvento = (eTipoEventoNFe?)item.TipoEvento,
                Sequencia = (int?)SequenciaEvento
            };

            if (!retornosPorEvento.TryGetValue(chaveRetorno, out var retorno))
            {
                return new ManifestacaoDestinatarioResultado(
                    item.ChaveNFe.Chave,
                    item.TipoEvento,
                    retornoSefaz.cStat,
                    retornoSefaz.xMotivo,
                    null,
                    null,
                    SituacaoManifestacaoDestinatario.Rejeitada);
            }

            return new ManifestacaoDestinatarioResultado(
                item.ChaveNFe.Chave,
                item.TipoEvento,
                retorno.cStat,
                retorno.xMotivo,
                retorno.nProt,
                retorno.dhRegEvento,
                ObterSituacao(retorno.cStat));
        }).ToList();
    }

    private static SituacaoManifestacaoDestinatario ObterSituacao(int codigoStatus)
    {
        return codigoStatus switch
        {
            StatusEventoRegistrado => SituacaoManifestacaoDestinatario.Confirmada,
            StatusDuplicidadeEvento => SituacaoManifestacaoDestinatario.ReconciliacaoPendente,
            _ => SituacaoManifestacaoDestinatario.Rejeitada
        };
    }

    private static void ValidarJustificativa(ManifestacaoDestinatarioItem item)
    {
        if (item.TipoEvento == eTipoEventoNFe.OperacaoNaoRealizada)
        {
            if (string.IsNullOrWhiteSpace(item.Justificativa) || item.Justificativa.Length < 15 || item.Justificativa.Length > 255)
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
}
