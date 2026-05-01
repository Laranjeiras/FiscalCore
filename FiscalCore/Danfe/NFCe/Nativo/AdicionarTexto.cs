using System.Drawing;
using System.Runtime.Versioning;

namespace FiscalCore.Danfe.NFCe.Nativo
{
    [SupportedOSPlatform("windows")]
    internal class AdicionarTexto
    {
        public static FontFamily? FontPadrao { get; set; }
        private readonly Graphics _graphics;
        private float _pontoX;
        private float _pontoY;
        private readonly SolidBrush? _br;

        public AdicionarTexto(Graphics graphics, string texto, int tamanhoFonte, SolidBrush? br = null, Font? font = null)
        {
            _graphics = graphics;
            Texto = texto;
            TamanhoFonte = tamanhoFonte;
            _br = br;

            if (font != null)
                Fonte = font;

            if (font == null && FontPadrao != null)
                Fonte = new Font(FontPadrao, TamanhoFonte);

            Medida = MedidasLinha.GetMedidas(this);
        }

        public void Desenhar(float pontoX, float pontoY)
        {
            _pontoX = pontoX;
            _pontoY = pontoY;

            if (_br != null)
            {
                _graphics.DrawString(Texto, Fonte, _br, new PointF(_pontoX, _pontoY));
                return;
            }

            _graphics.DrawString(Texto, Fonte, Brushes.Black, new PointF(_pontoX, _pontoY));
        }

        public Medida Medida { get; private set; } = null!;
        public string Texto { get; private set; } = null!;
        public Font Fonte { get; private set; } = null!;
        public int TamanhoFonte { get; private set; }
    }
}
