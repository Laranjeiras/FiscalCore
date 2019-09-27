namespace FiscalCore.Main.Configuracoes
{
    public class ConfiguracaoCsc
    {
        public ConfiguracaoCsc(string cIdToken, string csc)
        {
            this.CIdToken = cIdToken;
            Csc = csc;
        }

        protected ConfiguracaoCsc()
        {
        }

        public string CIdToken { get; set; }

        public string Csc { get; set; }
    }
}
