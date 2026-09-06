using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FiscalCore.Utils
{
    public static class Zip
    {
        public static byte[] Compress(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        /// <summary>
        /// Teto para o conteudo descomprimido. Um XML de DFe legitimo fica muito abaixo
        /// disso; o limite existe para conter payload malicioso que expande em ordens de
        /// grandeza (zip bomb).
        /// </summary>
        private const int TamanhoMaximoDescomprimidoBytes = 32 * 1024 * 1024;

        public static string Decompress(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso, TamanhoMaximoDescomprimidoBytes);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            CopyTo(src, dest, null);
        }

        private static void CopyTo(Stream src, Stream dest, int? limiteBytes)
        {
            byte[] bytes = new byte[4096];

            int cnt;
            long total = 0;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                total += cnt;

                if (limiteBytes.HasValue && total > limiteBytes.Value)
                    throw new InvalidOperationException(
                        "Conteudo descomprimido excede o limite de " +
                        (limiteBytes.Value / (1024 * 1024)) + " MB.");

                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
