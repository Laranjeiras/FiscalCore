using FiscalCore.Configuracoes;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using FiscalCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using AlgoPlus.Storage.Services;
using System.IO;
using FiscalCore.NotaFiscal.RetornoServicos.Autorizacao;
using FiscalCore.NotaFiscal;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4 : BaseSefazServico<AutorizarNFe4>, IAutorizarNFeServico
    {
        public AutorizarNFe4(ConfiguracaoServico cfgServico, ITransmitirSefazCommand transmitir, IStorageContext storageContext, ILogger<AutorizarNFe4> logger)
            : base(cfgServico, transmitir, logger, storageContext)
        {
        }

        public async Task<IRetornoAutorizacao> Autorizar(NFe nfe, CancellationToken cancellation, int idLote = 0)
        {
            logger.LogDebug("RECEBENDO NFe", nfe);
            var lista = new List<NFe> { nfe };
            return  await Autorizar(lista, cancellation, idLote);
        }

        public async Task<IRetornoAutorizacao> Autorizar(IList<NFe> nfes, CancellationToken cancellation, int idLote = 0)
        {
            logger.LogDebug($"RECEBENDO LISTA DE NFes TOTAL: {nfes.Count}");

            logger.LogDebug($"TRATAR NFes");

            new TratarNFeAutorizacao(ref nfes, configuracao)
                .Tratar();

            logger.LogDebug($"NFes TRATADAS");
            logger.LogDebug($"VALIDAR NFes");

            new ValidarNFeAutorizacao(nfes, configuracao)
                .Validar();

            logger.LogDebug($"NFes VALIDADAS");

            if (idLote <= 0)
                idLote = new Random().Next(10000000, 99999999);

            logger.LogInformation($"NUMERO LOTE: {idLote}");

            var nfesAssinadas = new List<NFe>();

            logger.LogInformation("ASSINAR NFes");

            foreach (var nfe in nfes)
            {
                var nfeAssinada = nfe.Assinar(configuracao.ConfigCertificado.Certificado);
                var xml = XmlUtils.ClasseParaXmlString<NFe>(nfeAssinada);
                xml = xml.Replace("xmlns=\"http://www.portalfiscal.inf.br/nfe\"", string.Empty);

                logger.LogInformation($"NFe [{nfeAssinada.infNFe.Id}] ASSINADA");

                if (configuracao.ValidarXmlSchema)
                {
                    ValidarXml(eTipoServico.AutorizarNFe, configuracao, xml);
                }

                logger.LogInformation($"NFe [{nfeAssinada.infNFe.Id}] VALIDADA");

                nfesAssinadas.Add(nfeAssinada);
            }

            var mod = nfes.Select(x => x.infNFe.ide.mod)
                .Distinct()
                .SingleOrDefault();
                
            var versaoServico = configuracao.VersaoAutorizacaoNFe.Descricao();
            var enviNFe = new enviNFe(versaoServico, idLote, eIndicadorSincronizacao.Sincrono, nfesAssinadas);

            var xmlEnviNFe = XmlUtils.ClasseParaXmlString<enviNFe>(enviNFe);
            var retorno = await Autorizar(xmlEnviNFe, mod, cancellation);
            return retorno;
        }

        private async Task<IRetornoAutorizacao> Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento, CancellationToken cancellation)
        {
            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-env-nfe.xml");
            await SalvarLog(arqEnv, xmlenviNFe4, cancellation);
            
            var urlSefaz = Fabrica.FabricarUrl.ObterUrl(eTipoServico.AutorizarNFe, configuracao.TipoAmbiente, modeloDocumento, configuracao.UF);
            logger.LogDebug($"URL SEFAZ OBTIDA {urlSefaz.Url}");

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.AutorizarNFe, xmlenviNFe4);

            logger.LogDebug($"TRANSMITIR SEFAZ");

            var retornoXmlString = await transmitir.TransmitirAsync(urlSefaz, envelope);
            logger.LogDebug($"RETORNO SEFAZ", retornoXmlString);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retEnviNFe").OuterXml;
                        
            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ret-env-nfe.xml");
            await SalvarLog(arqRet, retornoLimpo, cancellation);           

            var retEnviNFe = new RetNFeAutorizacao4(retornoLimpo);
            retEnviNFe.XmlEnviado = xmlenviNFe4;
            return retEnviNFe;
        }
    }
}
