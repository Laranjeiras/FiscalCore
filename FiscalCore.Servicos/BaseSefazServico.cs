using FiscalCore.Configuracoes;

namespace FiscalCore.Servicos
{
    public abstract class BaseSefazServico
    {
        public BaseSefazServico(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir)
        {
            this.Configuracao = configuracao;
            this.Transmitir = transmitir;
        }

        public ConfiguracaoServico Configuracao { get; }
        public ITransmitirSefazCommand Transmitir { get; }
    }
}
