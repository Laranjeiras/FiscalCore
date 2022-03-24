using System;
using System.Globalization;
using System.Linq;

namespace FiscalCore.Utils
{
    public static class Numero
    {
        public static decimal Arredondar(this decimal valor, int casasDecimais)
        {
            var valorNovo = decimal.Round(valor, casasDecimais, MidpointRounding.AwayFromZero);
            var valorNovoStr = valorNovo.ToString("F" + casasDecimais, CultureInfo.CurrentCulture);
            return decimal.Parse(valorNovoStr);
        }

        public static decimal? Arredondar(this decimal? valor, int casasDecimais)
        {
            if (valor == null) return null;
            return Arredondar(valor.Value, casasDecimais);
        }
    }
}
