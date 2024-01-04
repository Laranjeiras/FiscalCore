using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System.Linq;
using AlgoPlus.Storage.Services;
using System.IO;
using FiscalCore.Exceptions;
using System.Threading;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class CancelarNFeServico : IEventoServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand transmitir;
        private readonly CancellationToken cancellation;
        private const string VERSAO = "1.00";
        private const string LOG_PATH = "1.00";

        public CancelarNFeServico(ConfiguracaoServico cfgServico, IStorageContext storage, ITransmitirSefazCommand transmitir)
        {
            this.cfgServico = cfgServico;
            this.storage = storage.GetStorage("FiscalCore");
            this.transmitir = transmitir;
            this.cancellation = new CancellationToken();
        }

        public async Task<retEnvEvento> Cancelar(InfoNFeCancelar infoNFe)
        {
            return await Cancelar(new List<InfoNFeCancelar> { infoNFe });
        }

        public async Task<retEnvEvento> Cancelar(IList<InfoNFeCancelar> infos) 
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
                    cfgServico.UF,
                    cfgServico.TipoAmbiente,
                    cfgServico.Emitente,
                    item.ChaveAcesso,
                    item.ProtocoloAutorizacao.Protocolo,
                    item.Justificativa, VERSAO
                );

                eventos.Add(evento);
            }

            return await Cancelar(eventos, modelo);
        }

        private eModeloDocumento ExtrairModelo(IList<InfoNFeCancelar> infos)
        {
            var modelos = infos.Select(x => x.ChaveAcesso.Modelo).Distinct().ToList();
            if (modelos.Count != 1)
                throw new FalhaValidacaoException("Lista de cancelamento deve conter somente 1 modelo");

            return modelos.FirstOrDefault();
        }

        private async Task<retEnvEvento> Cancelar(List<evento> eventos, eModeloDocumento modeloDoc) 
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

                var _certificado = cfgServico.ConfigCertificado.Certificado;
                eventoTmp.Assinar(_certificado, cfgServico.ConfigCertificado.SignatureMethodSignedXml, cfgServico.ConfigCertificado.DigestMethodReference);
            }

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            var arqEnv = Path.Combine(LOG_PATH, $"{DateTime.Now.Ticks}-ped-eve.xml");
            await storage.SaveAsync(arqEnv, xmlEvento, cancellation);

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.CancelarNFe, cfgServico.TipoAmbiente, modeloDoc, cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CancelarNFe, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine(LOG_PATH, $"{DateTime.Now.Ticks}-ret-eve.xml");
            await storage.SaveAsync(arqRet, retornoXmlStringLimpa, cancellation);

            var retEnvEvento = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }

    public class InfoNFeCancelar
    {
        private const string JUSTIFICATIVA = "Nota Fiscal Emitida Indevidamente";
        public ChaveFiscal ChaveAcesso { get; private set; }
        public ProtocoloAutorizacao ProtocoloAutorizacao { get; private set; }
        public string Justificativa { get; private set; }

        public InfoNFeCancelar(ChaveFiscal chaveAcesso, ProtocoloAutorizacao protocoloAutorizacao, string justificativa = JUSTIFICATIVA)
        {
            ChaveAcesso = chaveAcesso ?? throw new ArgumentNullException(nameof(chaveAcesso));
            ProtocoloAutorizacao = protocoloAutorizacao ?? throw new ArgumentNullException(nameof(protocoloAutorizacao));

            if (string.IsNullOrEmpty(justificativa) || justificativa.Length < 15 || justificativa.Length > 255)
                throw new Exception("Justificativa de conter entre 15 e 255 caracteres");

            Justificativa = justificativa;
        }
    }
}
