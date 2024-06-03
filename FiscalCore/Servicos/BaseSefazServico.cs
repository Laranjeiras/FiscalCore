using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using FiscalCore.Validacoes;

namespace FiscalCore.Servicos
{
    public abstract class BaseSefazServico<T> : BaseServico<T> where T: BaseSefazServico<T>
    {
        protected readonly ConfiguracaoServico configuracao;

        public BaseSefazServico(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir, ILogger<T> logger, IStorageContext storageContext) 
            : base(storageContext, logger, transmitir)
        {
            configuracao.Validar();
            this.configuracao = configuracao;
        }
    }

    public abstract class BaseSefazServicoBasico<T> : BaseServico<T> where T: BaseSefazServicoBasico<T>
    {
        protected readonly ConfiguracaoBasicaServico configuracao;
        public BaseSefazServicoBasico(ConfiguracaoBasicaServico configuracao, ITransmitirSefazCommand transmitir, ILogger<T> logger, IStorageContext storageContext)
            : base(storageContext, logger, transmitir)
        {
            configuracao.Validar();
            this.configuracao = configuracao;
        }
    }

    public abstract class BaseServico<T> where T : class
    {
        protected readonly IStorageContext storageContext;
        protected readonly ILogger<T> logger;
        protected readonly ITransmitirSefazCommand transmitir;
        private const string STORAGE_NAME = "FiscalCore";

        protected BaseServico(IStorageContext storageContext, ILogger<T> logger, ITransmitirSefazCommand transmitir)
        {
            this.storageContext = storageContext;
            this.logger = logger;
            this.transmitir = transmitir;
        }

        protected async Task SalvarLog(string filename, string conteudo, CancellationToken cancellation)
        {
            try
            {
                logger?.LogDebug($"SALVAR LOG XML {filename}");
                var storage = storageContext.GetStorage(STORAGE_NAME);
                var fileInfo = await storage.SaveAsync(filename, conteudo, cancellation);
                logger?.LogDebug($"LOG SALVO {fileInfo.AbsolutePath}");
            }
            catch (Exception ex)
            {
                logger.LogWarning("OCORREU UM ERRO AO SALVAR O LOG (STORAGE: FiscalCore) | {}", ex.Message);
            }
        }
        
        protected void ValidarXml(eTipoServico tipoServico, ConfiguracaoServico config, string xmlValidar)
        {
            var validacao = new ValidarXml(tipoServico, config);
            ValidarXml(validacao, xmlValidar);
        }

        protected void ValidarXml(eTipoServico tipoServico, ConfiguracaoBasicaServico config, string xmlValidar)
        {
            var validacao = new ValidarXml(tipoServico, config);
            ValidarXml(validacao, xmlValidar);
        }

        private void ValidarXml(ValidarXml validacao, string xmlValidar)
        {
            validacao.Validar(xmlValidar);
            if (!validacao.Valido)
            {
                throw new FalhaValidacaoException(validacao.ToString());
            }
        }
    }
}
