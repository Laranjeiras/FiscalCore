using FiscalCore.Configuracoes;
using FiscalCore.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using FiscalCore.Validacoes;

namespace FiscalCore.Servicos
{
    public abstract class BaseSefazServico<T> : BaseServico<T> where T: BaseSefazServico<T>
    {
        protected readonly ConfiguracaoServico configuracao;

        public BaseSefazServico(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir, ILogger<T> logger)
            : base(logger, transmitir)
        {
            configuracao.Validar();
            this.configuracao = configuracao;
        }

        protected override string GetDiretorioSalvarXml() => configuracao.DiretorioSalvarXml;
    }

    public abstract class BaseSefazServicoBasico<T> : BaseServico<T> where T: BaseSefazServicoBasico<T>
    {
        protected readonly ConfiguracaoBasicaServico configuracao;

        public BaseSefazServicoBasico(ConfiguracaoBasicaServico configuracao, ITransmitirSefazCommand transmitir, ILogger<T> logger)
            : base(logger, transmitir)
        {
            configuracao.Validar();
            this.configuracao = configuracao;
        }

        protected override string GetDiretorioSalvarXml() => Directory.GetCurrentDirectory();
    }

    public abstract class BaseServico<T> where T : class
    {
        protected readonly ILogger<T> logger;
        protected readonly ITransmitirSefazCommand transmitir;

        protected BaseServico(ILogger<T> logger, ITransmitirSefazCommand transmitir)
        {
            this.logger = logger;
            this.transmitir = transmitir;
        }

        protected abstract string GetDiretorioSalvarXml();

        protected async Task SalvarLog(string filename, string conteudo, CancellationToken cancellation)
        {
            try
            {
                logger?.LogDebug($"SALVAR LOG XML {filename}");
                var diretorio = GetDiretorioSalvarXml();
                var storage = new FileStorage(diretorio);
                var fileInfo = await storage.SaveAsync(filename, conteudo, cancellation);
                logger?.LogDebug($"LOG SALVO {fileInfo.FullName}");
            }
            catch (Exception ex)
            {
                logger?.LogWarning("OCORREU UM ERRO AO SALVAR O LOG (STORAGE: FiscalCore) | {}", ex.Message);
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
