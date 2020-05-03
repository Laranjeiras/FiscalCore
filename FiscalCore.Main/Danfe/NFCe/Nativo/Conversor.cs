using System;
using System.Security.Cryptography;
using System.Text;

namespace FiscalCore.Main.Danfe.NFCe.Nativo
{
    internal static class Conversor
    {
        /// <summary>
        /// Obtém uma string Hexadecimal de uma string passada no parâmetro
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ObterHexDeString(string s)
        {
            var hex = "";
            foreach (var c in s)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        /// <summary>
        /// Obtém uma <see cref="string"/> SHA1, no formato hexadecimal da <see cref="string"/> passada no parâmero        
        /// </summary>
        public static string ObterHexSha1DeString(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return ObterHexDeByteArray(hashBytes);
        }

        /// <summary>
        /// Obtém uma string Hexadecimal do array de bytes passado no parâmetro
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ObterHexDeByteArray(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
