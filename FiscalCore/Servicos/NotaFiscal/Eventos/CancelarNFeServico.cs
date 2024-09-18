using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using System.Linq;
using AlgoPlus.Storage.Services;
using System.IO;
using FiscalCore.Exceptions;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class CancelarNFeServico : BaseSefazServico<CancelarNFeServico>, IEventoServico
    {
        private const string VERSAO = "1.00";
        private const string LOG_PATH = "1.00";

        public CancelarNFeServico(ConfiguracaoServico cfgServico, IStorageContext storage, ITransmitirSefazCommand transmitir, ILogger<CancelarNFeServico> logger)
            :base(cfgServico, transmitir, logger, storage)
        {
        }

        public async Task<retEnvEvento> Cancelar(InfoNFeCancelar infoNFe, CancellationToken cancellation) =>
            await Cancelar(new List<InfoNFeCancelar> { infoNFe }, cancellation);

        public async Task<retEnvEvento> Cancelar(IList<InfoNFeCancelar> infos, CancellationToken cancellation) 
        {
            if (infos == null || infos.Count <= 0)
                throw new Exception("Informações da NFe não encontrada");

            if (infos.Count > 20)
                throw new Exception("No máximo 20 NFes podem ser canceladas");

            var modelo = ExtrairModelo(infos);

            List<evento> eventos = new List<evento>();

            foreach (var item in infos)
            {
                var evento = FiscalCore.Modelos.Eventos.evento.CriarEventoCancelamento(
                    configuracao.UF,
                    configuracao.TipoAmbiente,
                    configuracao.Emitente,
                    item.ChaveAcesso,
                    item.ProtocoloAutorizacao.Protocolo,
                    item.Justificativa, VERSAO
                );

                eventos.Add(evento);
            }

            return await Cancelar(eventos, modelo, cancellation);
        }

        private eModeloDocumento ExtrairModelo(IList<InfoNFeCancelar> infos)
        {
            var modelos = infos.Select(x => x.ChaveAcesso.Modelo).Distinct().ToList();
            if (modelos.Count != 1)
                throw new FalhaValidacaoException("Lista de cancelamento deve conter somente 1 modelo");

            return modelos.FirstOrDefault();
        }

        private async Task<retEnvEvento> Cancelar(List<evento> eventos, eModeloDocumento modeloDoc, CancellationToken cancellation) 
        {
            var pedEvento = new envEvento
            {
                versao = VERSAO,
                idLote = 1,
                evento = eventos
            };

            foreach (var eventoTmp in eventos)
            {
                eventoTmp.infEvento.Id = "ID" + ((int)eventoTmp.infEvento.tpEvento) + eventoTmp.infEvento.chNFe +
                                      eventoTmp.infEvento.nSeqEvento.ToString().PadLeft(2, '0');

                var _certificado = configuracao.ConfigCertificado.Certificado;
                eventoTmp.Assinar(_certificado, configuracao.ConfigCertificado.SignatureMethodSignedXml, configuracao.ConfigCertificado.DigestMethodReference);
            }

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ped-eve.xml");
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.CancelarNFe, configuracao.TipoAmbiente, modeloDoc, configuracao.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CancelarNFe, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ret-eve.xml");
            await SalvarLog(arqRet, retornoXmlStringLimpa, cancellation);

            var retEnvEvento = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}
