using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace FiscalCore.Servicos.NotaFiscal.Autorizacao
{
    public class AutorizarNFe : BaseSefazServicoBasico<AutorizarNFe>
    {
        private const string STORAGE_NAME = "FiscalCore";
        private IStorage storage;
        protected CancellationToken cancellation;

        public AutorizarNFe(ConfiguracaoBasicaServico configuracao, ITransmitirSefazCommand transmitir, ILogger<AutorizarNFe4> logger, IStorageContext storageContext) : base(configuracao, transmitir, logger, storageContext)
        {
            this.storage = storageContext.GetStorage(STORAGE_NAME);
        }

        public async Task<IRetornoAutorizacao> Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento, CancellationToken cancellation)
        {
            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-env-nfe.xml");
            await SalvarLog(arqEnv, xmlenviNFe4, cancellation);

            var urlSefaz = Fabrica.FabricarUrl.ObterUrl(eTipoServico.AutorizarNFe, configuracao.TipoAmbiente, modeloDocumento, configuracao.UF);
            logger?.LogDebug($"URL SEFAZ OBTIDA {urlSefaz.Url}");

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.AutorizarNFe, xmlenviNFe4);

            logger?.LogDebug($"TRANSMITIR SEFAZ");

            var retornoXmlString = await transmitir.TransmitirAsync(urlSefaz, envelope!);
            logger?.LogDebug("RETORNO SEFAZ {Retorno}", retornoXmlString);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retEnviNFe").OuterXml;

            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-ret-env-nfe.xml");
            await SalvarLog(arqRet, retornoLimpo, cancellation);

            var retEnviNFe = new RetNFeAutorizacao4(retornoLimpo);
            retEnviNFe.XmlEnviado = xmlenviNFe4;
            return retEnviNFe;
        }

        private new async Task SalvarLog(string filename, string conteudo, CancellationToken cancellation)
        {
            try
            {
                logger.LogInformation($"SALVAR LOG XML {filename}");
                var fileInfo = await storage.SaveAsync(filename, conteudo, cancellation);
                logger.LogInformation($"LOG SALVO {fileInfo.AbsolutePath}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OCORREU UM ERRO AO SALVAR ARQUIVO NO STORAGE.");

                if (!configuracao.IgnorarErroDeStorage)
                {
                    throw;
                }
            }
        }
    }
}
