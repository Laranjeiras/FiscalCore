using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Extensions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class EventoAdquirenteRtcServico : BaseSefazServico<EventoAdquirenteRtcServico>
    {
        private const string VERSAO = "1.00";

        public EventoAdquirenteRtcServico(
            ConfiguracaoServico cfgServico,
            IStorageContext storageContext,
            ITransmitirSefazCommand transmitir,
            ILogger<EventoAdquirenteRtcServico> logger)
            : base(cfgServico, transmitir, logger, storageContext)
        {
        }

        public async Task<retEnvEvento> TransmitirEventoAsync(InfoEventoAdquirenteRtc info, CancellationToken cancellation = default)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info), "Informações do evento não encontradas");

            if (string.IsNullOrWhiteSpace(info.ChaveAcesso))
                throw new FalhaValidacaoException("Chave de acesso é obrigatória");

            if (info.TipoEvento != eTipoEventoNFe.AceiteDebitoNFeCredito && info.TipoEvento != eTipoEventoNFe.ImobilizacaoItemNFe)
                throw new FalhaValidacaoException($"Tipo de evento não suportado para adquirente RTC: {info.TipoEvento}");

            if (info.nSeqEvento <= 0)
                throw new FalhaValidacaoException("Sequência do evento deve ser maior que zero");

            var chave = info.ChaveAcesso.Replace("NFe", "");

            var verAplic = !string.IsNullOrWhiteSpace(info.VersaoAplicativo)
                ? info.VersaoAplicativo
                : (!string.IsNullOrWhiteSpace(this.configuracao.VersaoProc) ? this.configuracao.VersaoProc : "1.00");

            var detEvento = new detEvento
            {
                versao = VERSAO,
                cOrgaoAutor = this.configuracao.UF,
                tpAutor = "2",
                verAplic = verAplic
            };

            if (info.TipoEvento == eTipoEventoNFe.AceiteDebitoNFeCredito)
            {
                if (string.IsNullOrWhiteSpace(info.IndAceitacao))
                    throw new FalhaValidacaoException("Indicador de aceitação (indAceitacao) é obrigatório para o evento 211128");

                if (info.IndAceitacao != "0" && info.IndAceitacao != "1")
                    throw new FalhaValidacaoException("Indicador de aceitação deve ser 0 (não aceite) ou 1 (aceite)");

                detEvento.descEvento = "Aceite de débito na apuração por emissão de nota de crédito";
                detEvento.indAceitacao = info.IndAceitacao;
            }

            if (info.TipoEvento == eTipoEventoNFe.ImobilizacaoItemNFe)
            {
                if (info.ItensImobilizados == null || info.ItensImobilizados.Count == 0)
                    throw new FalhaValidacaoException("Evento de imobilização exige ao menos um item em gImobilizacao");

                detEvento.descEvento = "Imobilização de Item";
                detEvento.gImobilizacao = info.ItensImobilizados;
            }

            var infEvento = new infEventoEnv
            {
                chNFe = chave,
                CNPJ = this.configuracao.Emitente!.CNPJ,
                CPF = this.configuracao.Emitente.CPF,
                cOrgao = this.configuracao.UF,
                dhEvento = DateTime.Now,
                nSeqEvento = info.nSeqEvento,
                tpAmb = this.configuracao.TipoAmbiente,
                tpEvento = info.TipoEvento,
                verEvento = VERSAO,
                detEvento = detEvento
            };

            var evento = new evento
            {
                versao = VERSAO,
                infEvento = infEvento
            };

            evento.infEvento.Id = "ID" + ((int)evento.infEvento.tpEvento) + evento.infEvento.chNFe +
                                  evento.infEvento.nSeqEvento.ToString().PadLeft(LayoutFiscal.TamanhoSequenciaEvento, '0');

            var certificado = this.configuracao.ConfigCertificado.Certificado;
            evento.Assinar(certificado, this.configuracao.ConfigCertificado.SignatureMethodSignedXml, this.configuracao.ConfigCertificado.DigestMethodReference);

            var idLote = info.IdLote > 0 ? info.IdLote : (info.nSeqEvento > 0 ? info.nSeqEvento : 1);
            var pedEvento = new envEvento
            {
                versao = VERSAO,
                idLote = idLote,
                evento = new List<evento> { evento }
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve-rtc.xml", this.configuracao));
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            var sefazUrl = SefazEndpointResolver.ObterUrlEvento(
                info.TipoEvento,
                this.configuracao.TipoAmbiente,
                eModeloDocumento.NFe,
                this.configuracao.UF);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CancelarNFe, xmlEvento);

            var retornoXmlString = await this.transmitir.TransmitirAsync(sefazUrl, envelope!, cancellation);
            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve-rtc.xml", this.configuracao));
            await SalvarLog(arqRet, retornoXmlStringLimpa, cancellation);

            var retorno = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);
            return retorno;
        }
    }
}
