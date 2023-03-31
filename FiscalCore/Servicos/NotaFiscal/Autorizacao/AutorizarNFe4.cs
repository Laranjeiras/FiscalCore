using FiscalCore.Configuracoes;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using FiscalCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using FiscalCore.Validacoes;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using AlgoPlus.Storage.Services;
using System.IO;
using FiscalCore.NotaFiscal.RetornoServicos.Autorizacao;
using FiscalCore.NotaFiscal;
using FiscalCore.Exceptions;
using Microsoft.Extensions.Logging;
using FiscalCore.Servicos.NotaFiscal;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4 : IAutorizarNFeServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly ITransmitirSefazCommand transmitir;
        private readonly IStorage storage;
        private readonly ILogger<NotaFiscalServico> logger;

        public AutorizarNFe4(ConfiguracaoServico cfgServico, ITransmitirSefazCommand transmitir, IStorageContext storage, ILogger<NotaFiscalServico> logger)
        {
            this.cfgServico = cfgServico;
            this.transmitir = transmitir;
            this.storage = storage.GetStorage("FiscalCore");
            this.logger = logger;
        }

        public async Task<IRetornoAutorizacao> Autorizar(NFe nfe, int idLote = 0)
        {
            logger.LogDebug("RECEBENDO NFe", nfe);
            var lista = new List<NFe> { nfe };
            return  await Autorizar(lista, idLote);
        }

        public async Task<IRetornoAutorizacao> Autorizar(IList<NFe> nfes, int idLote = 0)
        {
            logger.LogDebug($"RECEBENDO LISTA DE NFes TOTAL: {nfes.Count}");

            logger.LogDebug($"TRATAR NFes");

            new TratarNFeAutorizacao(ref nfes, cfgServico)
                .Tratar();

            logger.LogDebug($"NFes TRATADAS");
            logger.LogDebug($"VALIDAR NFes");

            new ValidarNFeAutorizacao(nfes, cfgServico)
                .Validar();

            logger.LogDebug($"NFes VALIDADAS");

            if (idLote <= 0)
                idLote = new Random().Next(10000000, 99999999);

            logger.LogInformation($"NUMERO LOTE: {idLote}");

            var nfesAssinadas = new List<NFe>();

            logger.LogInformation("ASSINAR NFes");

            foreach (var nfe in nfes)
            {
                var nfeAssinada = nfe.Assinar(cfgServico.ConfigCertificado.Certificado);
                var xml = XmlUtils.ClasseParaXmlString<NFe>(nfeAssinada);
                xml = xml.Replace("xmlns=\"http://www.portalfiscal.inf.br/nfe\"", string.Empty);

                logger.LogInformation($"NFe [{nfeAssinada.infNFe.Id}] ASSINADA");

                var validacao = new ValidarXml(eTipoServico.AutorizarNFe, cfgServico);
                validacao.Validar(xml);
                if (!validacao.Valido)
                {   
                    throw new FalhaValidacaoException(validacao.ToString());
                }

                logger.LogInformation($"NFe [{nfeAssinada.infNFe.Id}] VALIDADA");

                nfesAssinadas.Add(nfeAssinada);
            }

            var mod = nfes.Select(x => x.infNFe.ide.mod)
                .Distinct()
                .SingleOrDefault();
                
            var versaoServico = cfgServico.VersaoAutorizacaoNFe.Descricao();
            var enviNFe = new enviNFe(versaoServico, idLote, eIndicadorSincronizacao.Sincrono, nfesAssinadas);

            var xmlEnviNFe = XmlUtils.ClasseParaXmlString<enviNFe>(enviNFe);
            var retorno = await Autorizar(xmlEnviNFe, mod);
            return retorno;
        }

        private async Task<IRetornoAutorizacao> Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento)
        {
            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-env-nfe.xml");
            await SalvarLog(arqEnv, xmlenviNFe4);
            
            var urlSefaz = Fabrica.FabricarUrl.ObterUrl(eTipoServico.AutorizarNFe, cfgServico.TipoAmbiente, modeloDocumento, cfgServico.UF);
            logger.LogDebug($"URL SEFAZ OBTIDA {urlSefaz.Url}");

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.AutorizarNFe, xmlenviNFe4);

            logger.LogDebug($"TRANSMITIR SEFAZ");

            var retornoXmlString = await transmitir.TransmitirAsync(urlSefaz, envelope);
            logger.LogDebug($"RETORNO SEFAZ", retornoXmlString);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retEnviNFe").OuterXml;
                        
            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ret-env-nfe.xml");
            await SalvarLog(arqRet, retornoLimpo);           

            var retEnviNFe = new RetNFeAutorizacao4(retornoLimpo);
            retEnviNFe.XmlEnviado = xmlenviNFe4;
            return retEnviNFe;
        }

        private async Task SalvarLog(string filename, string conteudo)
        {
            logger.LogInformation($"SALVAR LOG XML {filename}");
            var fileInfo = await storage.SaveAsync(filename, conteudo);
            logger.LogInformation($"LOG SALVO {fileInfo.AbsolutePath}");
        }
    }
}
