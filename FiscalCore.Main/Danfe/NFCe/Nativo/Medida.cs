namespace FiscalCore.Main.Danfe.NFCe.Nativo
{
    internal class Medida
    {
        public Medida(int altura, int largura)
        {
            Altura = altura;
            Largura = largura;
        }

        public int Altura { get; private set; }
        public int Largura { get; private set; }
    }
}
