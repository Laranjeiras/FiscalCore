using FiscalCore.Configuracoes;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class ComprovanteEntregaNFeServico : BaseSefazServico, IEventoServico
    {
        public ComprovanteEntregaNFeServico(ConfiguracaoServico configuracoes, ITransmitirSefazCommand transmitir) : base(configuracoes, transmitir)
        {
        }
    }
}
