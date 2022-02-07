using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using FiscalCore.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe
{
    public class ManifestacaoDestinatarioServico
    {
        private readonly ConfiguracaoServico config;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand transmitir;
        private readonly int nSeqEvento;

        public ManifestacaoDestinatarioServico(ConfiguracaoServico config, IStorage storage, ITransmitirSefazCommand transmitir)
        {
            this.config = config;
            this.storage = storage;
            this.transmitir = transmitir;
            this.nSeqEvento = 1;
        }

        public async Task<string> ManifestarAsync(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, string justificativa = null)
        {
            if(tipoEvento != eTipoEventoNFe.CienciaOperacao 
                && tipoEvento != eTipoEventoNFe.ConfirmacaoOperacao
                && tipoEvento != eTipoEventoNFe.DesconhecimentoOperacao
                && tipoEvento != eTipoEventoNFe.OperacaoNaoRealizada)
                    throw new Exception("Evento não permitido nesse serviço");

            var xmlEvento = GerarXmlEvento(chaveNFe.Chave, tipoEvento, justificativa);

            await storage.SaveAsync($"{DateTime.Now.Ticks}-ped-eve.xml", xmlEvento);

            Schemas.ValidarSchema(eTipoServico.ManifestacaoDestinatario, xmlEvento, config);
            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, config.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var xmlRetorno = await transmitir.TransmitirAsync(sefazUrl, envelope);
            var xmlRetLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;

            await storage.SaveAsync($"{DateTime.Now.Ticks}-ret-eve.xml", xmlRetLimpo);

            return xmlRetLimpo;
        }

        private string GerarXmlEvento(string chaveAcesso, eTipoEventoNFe tipoEvento, string justificativa = null)
        {
            if (tipoEvento == eTipoEventoNFe.OperacaoNaoRealizada && justificativa == null)
                throw new ArgumentNullException("Justificativa deve ser informada");
            
            if (tipoEvento != eTipoEventoNFe.OperacaoNaoRealizada)
                justificativa = null;

            var infEvento = new infEventoEnv
            {
                chNFe = chaveAcesso,
                CNPJ = config.Emitente.CNPJ,
                CPF = config.Emitente.CPF,
                cOrgao = eUF.AN,
                dhEvento = DateTime.Now,
                nSeqEvento = 1,
                tpAmb = config.TipoAmbiente,
                tpEvento = eTipoEventoNFe.CienciaOperacao,
                verEvento = config.VersaoManifestacaoDestinatario.Descricao(),
                Id = "ID" + ((int)tipoEvento) + chaveAcesso + nSeqEvento.ToString().PadLeft(2, '0'),                
                detEvento = new detEvento()
                {
                    versao = config.VersaoManifestacaoDestinatario.Descricao(),
                    descEvento = tipoEvento.Descricao().RemoverAcentos()
                }
            };

            var evento = new evento
            {
                versao = config.VersaoManifestacaoDestinatario.Descricao(),
                infEvento = infEvento
            };

            evento.Assinar(ObterCertificado.Obter(config.ConfigCertificado), config.ConfigCertificado.SignatureMethodSignedXml, config.ConfigCertificado.DigestMethodReference);

            var pedEvento = new envEvento
            {
                versao = config.VersaoManifestacaoDestinatario.Descricao(),
                idLote = 1,
                evento = new List<evento> { evento }
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);
            return xmlEvento;
        }
    }
}
