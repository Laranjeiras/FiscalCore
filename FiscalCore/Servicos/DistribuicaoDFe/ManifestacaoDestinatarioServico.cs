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
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe
{
    public class ManifestacaoDestinatarioServico : BaseSefazServico<ManifestacaoDestinatarioServico>
    {
        private readonly int nSeqEvento;

        public ManifestacaoDestinatarioServico(ConfiguracaoServico config, IStorageContext storageContext, ITransmitirSefazCommand transmitir, ILogger<ManifestacaoDestinatarioServico> logger)
            :base(config, transmitir, logger, storageContext)
        {
            this.nSeqEvento = 1;
        }

        public async Task<retEnvEvento> ManifestarAsync(ChaveFiscal chaveNFe, eTipoEventoNFe tipoEvento, string justificativa, CancellationToken cancellation)
        {
            logger.LogInformation("INICIANDO MANIFESTACAO DO DESTINATARIO");

            if (!PodeManifestar(tipoEvento))
                throw new Exception("Evento não permitido nesse serviço");

            var xmlEvento = GerarXmlEvento(chaveNFe.Chave, tipoEvento, justificativa);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve.xml", configuracao));
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            if (configuracao.ValidarXmlSchema)
            {
                ValidarXml(eTipoServico.ManifestacaoDestinatario, configuracao, xmlEvento);
            }

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var xmlRetorno = await transmitir.TransmitirAsync(sefazUrl, envelope);
            var xmlRetLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve.xml", configuracao));
            await SalvarLog(arqEnv, xmlRetLimpo, cancellation);

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
                CNPJ = configuracao.Emitente.CNPJ,
                CPF = configuracao.Emitente.CPF,
                cOrgao = eUF.AN,
                dhEvento = DateTime.Now,
                nSeqEvento = 1,
                tpAmb = configuracao.TipoAmbiente,
                tpEvento = tipoEvento,
                verEvento = configuracao.VersaoManifestacaoDestinatario.Descricao(),
                Id = "ID" + ((int)tipoEvento) + chaveAcesso + nSeqEvento.ToString().PadLeft(2, '0'),                
                detEvento = new detEvento()
                {
                    versao = configuracao.VersaoManifestacaoDestinatario.Descricao(),
                    descEvento = tipoEvento.Descricao().RemoverAcentos()
                }
            };

            evento evento = evento.CriarEvento(configuracao.VersaoManifestacaoDestinatario.Descricao(), infEvento);

            evento.Assinar(configuracao.ConfigCertificado.Certificado, configuracao.ConfigCertificado.SignatureMethodSignedXml, configuracao.ConfigCertificado.DigestMethodReference);

            envEvento pedEnvEvento = envEvento.Criar(configuracao.VersaoManifestacaoDestinatario.Descricao(), 1, evento);

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEnvEvento);
            return xmlEvento;
        }

        private bool PodeManifestar(eTipoEventoNFe tipoEvento) => tipoEvento == eTipoEventoNFe.CienciaOperacao
                || tipoEvento == eTipoEventoNFe.ConfirmacaoOperacao
                || tipoEvento == eTipoEventoNFe.DesconhecimentoOperacao
                || tipoEvento == eTipoEventoNFe.OperacaoNaoRealizada;


    }
}
