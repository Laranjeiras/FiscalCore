namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class InfoCartaCorrecao 
    {
        public InfoCartaCorrecao(string chaveAcesso, string correcao, int nSequenciaEvento)
        {
            ChaveAcesso = chaveAcesso;
            Correcao = correcao;
            nSeqEvento = nSequenciaEvento;
        }

        public string ChaveAcesso { get; private set; }

        public string Correcao { get; private set; }

        public int nSeqEvento { get; private set; }
    }
}
