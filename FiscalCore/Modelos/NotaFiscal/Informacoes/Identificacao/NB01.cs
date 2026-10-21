namespace FiscalCore.NotaFiscal.Informacoes.Identificacao
{
    public class NB01
    {
        public decimal? vICMSPrevisto { get; set; }

        public bool ShouldSerializevICMSPrevisto()
        {
            return vICMSPrevisto.HasValue;
        }
    }
}
