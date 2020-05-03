using System;
using System.Drawing;

namespace FiscalCore.Main.Danfe.NFCe.Nativo
{
    internal static class MedidasLinha
    {
        public static Medida GetMedidas(AdicionarTexto adicionarTexto)
        {
            Medida medida = ObterMedidas(adicionarTexto.Texto, adicionarTexto.Fonte);

            return medida;
        }

        public static Medida ObterMedidas(string texto, Font fonte)
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            SizeF tamanhoDaString = g.MeasureString(texto, fonte);
            int alturaLinha = Convert.ToInt32(tamanhoDaString.Height);
            int larguraLinha = Convert.ToInt32(tamanhoDaString.Width);

            return new Medida(alturaLinha, larguraLinha);
        }

    }
}
