using FiscalCore.Configuracoes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FiscalCore.Utils
{
    public static class Arquivo
    {
        public static async Task<string> SalvarArquivoAsync(ConfiguracaoServico configuracao, string subDiretorio, string nomeArquivo, string conteudo, DateTime? dataFiscal = null)
        {
            var dir = Path.Combine(configuracao.DiretorioSalvarXml, subDiretorio);
            if (dataFiscal != null)
                dir = Path.Combine(dir, dataFiscal?.ToString("MM_yyyy"));

            return await SalvarArquivoAsync(dir, nomeArquivo, conteudo);
        }

        public static async Task<string> SalvarArquivoAsync(ConfiguracaoServico configuracao, string nomeArquivo, string conteudo, DateTime? dataFiscal = null)
        {
            var dir = Path.Combine(configuracao.DiretorioSalvarXml);
            if (dataFiscal != null)
                dir = Path.Combine(dir, dataFiscal?.ToString("MM_yyyy"));

            return await SalvarArquivoAsync(dir, nomeArquivo, conteudo);
        }

        public static async Task<string> SalvarArquivoAsync(string dir, string nomeArquivo, string conteudo)
        {
            if (dir == null)
                throw new ArgumentNullException(nameof(dir), "O caminho do arquivo deve ser informado");
            if (nomeArquivo == null)
                throw new ArgumentNullException(nameof(nomeArquivo), "O nome do arquivo deve ser informado");
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "O conteúdo do arquivo deve ser informado");

            CriarDiretorioSeNaoExistir(dir);

            var filename = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(filename);
            await stw.WriteLineAsync(conteudo);
            stw.Close();
            return filename;
        }

        public static string CriarDiretorioSeNaoExistir(string diretorio)
        {
            if (diretorio == null)
                throw new ArgumentNullException(nameof(diretorio));

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            return diretorio;
        }
    }
}
