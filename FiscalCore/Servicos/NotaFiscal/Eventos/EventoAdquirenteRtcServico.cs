using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Extensions;
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
        private readonly ConfiguracaoServico cfgServico;
        private readonly new ITransmitirSefazCommand transmitir;
        private const string VERSAO = "1.00";

        public EventoAdquirenteRtcServico(
            ConfiguracaoServico cfgServico,
            IStorageContext storageContext,
            ITransmitirSefazCommand transmitir,
            ILogger<EventoAdquirenteRtcServico> logger)
            : base(cfgServico, transmitir, logger, storageContext)
        {
            this.cfgServico = cfgServico;
            this.transmitir = transmitir;
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

            var detEvento = new detEvento
            {
                versao = VERSAO,
                cOrgaoAutor = cfgServico.UF,
                tpAutor = "2",
                verAplic = "1.00"
            };

            if (info.TipoEvento == eTipoEventoNFe.AceiteDebitoNFeCredito)
            {
                detEvento.descEvento = "Aceite de débito na apuração por emissão de nota de crédito";
                detEvento.indAceitacao = string.IsNullOrEmpty(info.IndAceitacao) ? "1" : info.IndAceitacao;
            }
            else
            {
                detEvento.descEvento = "Imobilização de Item";
                if (info.ItensImobilizados == null || info.ItensImobilizados.Count == 0)
                    throw new FalhaValidacaoException("Evento de imobilização exige ao menos um item em gImobilizacao");
                detEvento.gImobilizacao = info.ItensImobilizados;
            }

            var infEvento = new infEventoEnv
            {
                chNFe = chave,
                CNPJ = cfgServico.Emitente!.CNPJ,
                CPF = cfgServico.Emitente.CPF,
                cOrgao = eUF.AN,
                dhEvento = DateTime.Now,
                nSeqEvento = info.nSeqEvento,
                tpAmb = cfgServico.TipoAmbiente,
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

            var certificado = cfgServico.ConfigCertificado.Certificado;
            evento.Assinar(certificado, cfgServico.ConfigCertificado.SignatureMethodSignedXml, cfgServico.ConfigCertificado.DigestMethodReference);

            var pedEvento = new envEvento
            {
                versao = VERSAO,
                idLote = 1,
                evento = new List<evento> { evento }
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve-rtc.xml", cfgServico));
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, cfgServico.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope!);
            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve-rtc.xml", cfgServico));
            await SalvarLog(arqRet, retornoXmlStringLimpa, cancellation);

            var retorno = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);
            return retorno;
        }
    }
}
