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
using DFeBR.EmissorNFe.Utilidade.Exceptions;
using AlgoPlus.Storage.Services;
using System.IO;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class CancelarNFeServico : IEventoServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand transmitir;
        string versao;

        public CancelarNFeServico(ConfiguracaoServico cfgServico, IStorage storage, ITransmitirSefazCommand transmitir)
        {
            this.cfgServico = cfgServico;
            this.storage = storage;
            this.transmitir = transmitir;
            versao = "1.00";
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

            var modelos = infos.Select(x => x.ChaveAcesso.Modelo).Distinct().ToList();
            if (modelos.Count != 1)
                throw new FalhaValidacaoException("Lista de cancelamento deve conter somente 1 modelo");

            eModeloDocumento modeloDocumento = modelos.FirstOrDefault();

            List<evento> eventos = new List<evento>();

            foreach (var item in infos)
            {
                var protocolo = item.ProtocoloAutorizacao;
                var chave = item.ChaveAcesso;
                var just = item.Justificativa;

                if(string.IsNullOrEmpty(just) || just.Length < 15 || just.Length > 255)
                    throw new Exception("Justificativa de conter entre 15 e 255 caracteres");

                var detEvento = new detEvento
                {
                    nProt = protocolo.Protocolo,
                    versao = versao,
                    xJust = just,
                    descEvento = "Cancelamento",
                };

                var infEvento = new infEventoEnv
                {
                    cOrgao = cfgServico.UF,
                    tpAmb = cfgServico.TipoAmbiente,
                    chNFe = chave.Chave,
                    dhEvento = DateTime.Now,
                    tpEvento = eTipoEventoNFe.NFeCancelamento,
                    nSeqEvento = 1,
                    verEvento = versao,
                    detEvento = detEvento
                };

                if (!string.IsNullOrEmpty(cfgServico.Emitente.CNPJ))
                    infEvento.CNPJ = cfgServico.Emitente.CNPJ;
                else
                    infEvento.CPF = cfgServico.Emitente.CPF;

                var evento = new evento { versao = versao, infEvento = infEvento };
                eventos.Add(evento);
            }            
            return await Cancelar(eventos, modeloDocumento);
        }

        private async Task<retEnvEvento> Cancelar(List<evento> eventos, eModeloDocumento modeloDoc) 
        {
            var pedEvento = new envEvento
            {
                versao = versao,
                idLote = 1,
                evento = eventos
            };

            foreach (var eventoTmp in eventos)
            {
                eventoTmp.infEvento.Id = "ID" + ((int)eventoTmp.infEvento.tpEvento) + eventoTmp.infEvento.chNFe +
                                      eventoTmp.infEvento.nSeqEvento.ToString().PadLeft(2, '0');

                var _certificado = ObterCertificado.Obter(cfgServico.ConfigCertificado);
                eventoTmp.Assinar(_certificado, cfgServico.ConfigCertificado.SignatureMethodSignedXml, cfgServico.ConfigCertificado.DigestMethodReference);
            }

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ped-eve.xml");
            await storage.SaveAsync(arqEnv, xmlEvento);

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.CancelarNFe, cfgServico.TipoAmbiente, modeloDoc, cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CancelarNFe, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ret-eve.xml");
            await storage.SaveAsync(arqRet, retornoXmlStringLimpa);

            var retEnvEvento = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }

    public class InfoNFeCancelar
    {
        public ChaveFiscal ChaveAcesso { get; private set; }
        public ProtocoloAutorizacao ProtocoloAutorizacao { get; private set; }
        public string Justificativa { get; private set; }

        public InfoNFeCancelar(ChaveFiscal chaveAcesso, ProtocoloAutorizacao protocoloAutorizacao, string justificativa = "Nota Fiscal Emitida Indevidamente")
        {
            ChaveAcesso = chaveAcesso ?? throw new ArgumentNullException(nameof(chaveAcesso));
            ProtocoloAutorizacao = protocoloAutorizacao ?? throw new ArgumentNullException(nameof(protocoloAutorizacao));
            Justificativa = justificativa;
        }
    }
}
