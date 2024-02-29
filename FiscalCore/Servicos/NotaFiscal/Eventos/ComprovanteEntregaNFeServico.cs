using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using Microsoft.Extensions.Logging;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class ComprovanteEntregaNFeServico : BaseSefazServico<ComprovanteEntregaNFeServico>, IEventoServico
    {
        public ComprovanteEntregaNFeServico(
            ConfiguracaoServico config,
            ITransmitirSefazCommand transmitir,
            ILogger<ComprovanteEntregaNFeServico> logger,
            IStorageContext storageContext) 
            : base(config, transmitir, logger, storageContext)
        {
        }
    }
}
