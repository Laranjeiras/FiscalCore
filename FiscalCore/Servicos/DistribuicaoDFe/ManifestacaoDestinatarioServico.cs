using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Extensions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using FiscalCore.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe
{
    public class ManifestacaoDestinatarioServico
    {
        private readonly ConfiguracaoServico config;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand transmitir;
        private readonly int nSeqEvento;
        private readonly CancellationToken cancellation;

        public ManifestacaoDestinatarioServico(ConfiguracaoServico config, IStorage storage, ITransmitirSefazCommand transmitir)
        {
            this.config = config;
            this.storage = storage;
            this.transmitir = transmitir;
            this.nSeqEvento = 1;
            this.cancellation = new CancellationToken();
        }

        public async Task<retEnvEvento> ManifestarAsync(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, string justificativa = null)
        {
            if(tipoEvento != eTipoEventoNFe.CienciaOperacao 
                && tipoEvento != eTipoEventoNFe.ConfirmacaoOperacao
                && tipoEvento != eTipoEventoNFe.DesconhecimentoOperacao
                && tipoEvento != eTipoEventoNFe.OperacaoNaoRealizada)
                    throw new Exception("Evento não permitido nesse serviço");

            var xmlEvento = GerarXmlEvento(chaveNFe.Chave, tipoEvento, justificativa);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve.xml", config));
            await storage.SaveAsync(arqEnv, xmlEvento, cancellation);

            await storage.SaveAsync($"{DateTime.Now.Ticks}-ped-eve.xml", xmlEvento, cancellation);

            var validacao = new ValidarXml(eTipoServico.ManifestacaoDestinatario, config);
            validacao.Validar(xmlEvento);
            if (!validacao.Valido)
                throw new FalhaValidacaoException(validacao.ToString());

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, config.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var xmlRetorno = await transmitir.TransmitirAsync(sefazUrl, envelope);
            var xmlRetLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve.xml", config));
            await storage.SaveAsync(arqRet, xmlRetLimpo, cancellation);

            var retEnvEvento = XmlUtils.XmlStringParaClasse<retEnvEvento>(xmlRetLimpo);
            return retEnvEvento;
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
                tpEvento = tipoEvento,
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

            evento.Assinar(config.ConfigCertificado.Certificado, config.ConfigCertificado.SignatureMethodSignedXml, config.ConfigCertificado.DigestMethodReference);

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
