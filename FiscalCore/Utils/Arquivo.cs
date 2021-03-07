using FiscalCore.Configuracoes;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FiscalCore.Utils
{
    public static class Arquivo
    {
        public static void SalvarArquivoAutorizado(DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.NFe nfe, IConfiguracaoServico _configServico, string conteudo)
        {
            var nomeArquivo = $"{nfe.infNFe.Id}.xml";
            var ambiente = nfe.infNFe.ide.tpAmb.ToString();
            var dataFiscal = nfe.infNFe.ide.dhEmi;

            SalvarArquivoString(_configServico.DiretorioSalvarXml, ambiente, "Autorizacao", dataFiscal, nomeArquivo ,conteudo);
        }

        public static void SalvarArquivoString(string diretorioBase, string ambiente, string subDiretorio, DateTimeOffset dataFiscal, string nomeArquivo, string conteudo)
        {
            if (diretorioBase == null)
                throw new ArgumentNullException(nameof(diretorioBase), "O caminho do arquivo deve ser informado");
            if (ambiente == null)
                throw new ArgumentNullException(nameof(ambiente), "O ambiente do arquivo deve ser informado");
            if (subDiretorio == null)
                throw new ArgumentNullException(nameof(ambiente), "O subdiretorio do arquivo deve ser informado");
            if (nomeArquivo == null)
                throw new ArgumentNullException(nameof(nomeArquivo), "O nome do arquivo deve ser informado");
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "O conteúdo do arquivo deve ser informado");

            var diretorio = Path.Combine(diretorioBase, ambiente, dataFiscal.ToString("MM_yyyy"), subDiretorio);

            CriarDiretorioSeNaoExistir(diretorio);

            var encodedText = Encoding.UTF8.GetBytes(conteudo);
            var c1 = Path.Combine(diretorio, nomeArquivo);
            using (var sourceStream = new FileStream(c1, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                sourceStream.Write(encodedText, 0, encodedText.Length);
                sourceStream.Close();
            }
        }

        public static string LerArquivo(string arquivo)
        {
            TextReader tr = new StreamReader(arquivo);
            var texto = tr.ReadToEnd();
            tr.Close();
            return texto;
        }

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
