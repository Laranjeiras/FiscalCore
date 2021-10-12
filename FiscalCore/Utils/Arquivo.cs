using FiscalCore.Configuracoes;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FiscalCore.Utils
{
    public static class Arquivo
    {
        public static async Task<string> SalvarArquivoAsync(ConfiguracaoServico configuracao, string subDiretorio, string nomeArquivo, string conteudo, DateTime? dataFiscal = null)
        {
            var dir = Path.Combine(configuracao.DiretorioSalvarXml, configuracao.TipoAmbiente.ToString(), subDiretorio);
            if (dataFiscal != null)
                dir = Path.Combine(dir, dataFiscal?.ToString("MM_yyyy"));

            CriarDiretorioSeNaoExistir(dir);

            if (dir == null)
                throw new ArgumentNullException(nameof(dir), "O caminho do arquivo deve ser informado");
            if (nomeArquivo == null)
                throw new ArgumentNullException(nameof(nomeArquivo), "O nome do arquivo deve ser informado");
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "O conteúdo do arquivo deve ser informado");

            var arquivo = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(arquivo);
            await stw.WriteLineAsync(conteudo);
            stw.Close();
            return arquivo;
        }

        public static async Task<string> SalvarArquivoAsync(ConfiguracaoServico configuracao, string nomeArquivo, string conteudo, DateTime? dataFiscal = null)
        {
            var dir = Path.Combine(configuracao.DiretorioSalvarXml, configuracao.TipoAmbiente.ToString());
            if (dataFiscal != null)
                dir = Path.Combine(dir, dataFiscal?.ToString("MM_yyyy"));

            CriarDiretorioSeNaoExistir(dir);

            if (dir == null)
                throw new ArgumentNullException(nameof(dir), "O caminho do arquivo deve ser informado");
            if (nomeArquivo == null)
                throw new ArgumentNullException(nameof(nomeArquivo), "O nome do arquivo deve ser informado");
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "O conteúdo do arquivo deve ser informado");

            var arquivo = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(arquivo);
            await stw.WriteLineAsync(conteudo);
            stw.Close();
            return arquivo;
        }

        public static async Task<string> SalvarArquivoAsync(string dir, string nomeArquivo, string xmlString)
        {
            CriarDiretorioSeNaoExistir(dir);

            var filename = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(filename);
            await stw.WriteLineAsync(xmlString);
            stw.Close();
            return filename;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <param name="diretorio">Diretorio para salvar o arquivo, diretório temp caso não informe</param>
        /// <returns></returns>
        public static async Task<string> SalvarEmArquivo(this string texto, string nomeArquivo, string diretorio = null)
        {
            if (nomeArquivo is null)
                nomeArquivo = Guid.NewGuid().ToString();
            if (diretorio is null)
                diretorio = Path.GetTempPath();

            var salvo = await SalvarArquivoAsync(diretorio, nomeArquivo, texto);
            return salvo;
        }

        public static T ArquivoXmlParaClasse<T>(string arquivo) where T : class
        {
            if (!File.Exists(arquivo))
                throw new FileNotFoundException(arquivo);

            var xml = File.ReadAllText(arquivo);
            var ser = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            using var sr = new StringReader(xml);
            return (T)ser.Deserialize(sr);
        }

        public static string CriarDiretorioNaRaizDoApp(string nomeDiretorio)
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeDiretorio);
            return CriarDiretorioSeNaoExistir(caminho);
        }

        public static string CriarDiretorioSeNaoExistir(string diretorio)
        {
            if (diretorio == null)
                throw new ArgumentNullException(nameof(diretorio));

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            return diretorio;
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    var bytes_0 = new byte[4096];

                    int cnt;

                    while ((cnt = gs.Read(bytes_0, 0, bytes_0.Length)) != 0)
                    {
                        mso.Write(bytes_0, 0, cnt);
                    }
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }
    }
}
