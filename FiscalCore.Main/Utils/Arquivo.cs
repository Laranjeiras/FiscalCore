using System;
using System.IO;

namespace FiscalCore.Main.Utils
{
    public static class Arquivo
    {
        public static string CriarDiretorioNaRaizDoApp(string nomeDiretorio)
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeDiretorio);
            return CriarDiretorioSeNaoExistir(caminho);
        }

        /// <summary>
        /// Criar diretório se não existir
        /// </summary>
        /// <param name="diretorio">Diretório a ser criado</param>
        /// <returns></returns>
        public static string CriarDiretorioSeNaoExistir(string diretorio)
        {
            //var separador = Path.DirectorySeparatorChar;
            if (diretorio == null)
                throw new ArgumentNullException("diretorio");

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            return diretorio;
        }
    }
}
