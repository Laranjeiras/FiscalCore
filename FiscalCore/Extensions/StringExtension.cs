using System;
using System.Linq;

namespace FiscalCore.Extensions
{
    public static class StringExtension
    {
        public static string RemoverAcentos(this string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return texto;
        }

        public static string SomenteNumeros(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : new string(value.Where(Char.IsDigit).ToArray());
        }
    }
}
