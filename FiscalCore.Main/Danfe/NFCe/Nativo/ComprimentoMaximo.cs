namespace FiscalCore.Main.Danfe.NFCe.Nativo
{
    internal class ComprimentoMaximo
    {
        private readonly int _valor;

        public ComprimentoMaximo(int comprimentoMaximo)
        {
            _valor = comprimentoMaximo;
        }

        public int GetComprimentoMaximo()
        {
            return _valor;
        }
    }
}
