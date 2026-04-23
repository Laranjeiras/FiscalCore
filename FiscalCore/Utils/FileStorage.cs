using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Utils
{
    public class FileStorage
    {
        private readonly string diretorio;

        public FileStorage(string diretorio)
        {
            this.diretorio = diretorio ?? Directory.GetCurrentDirectory();
            EnsureDirectoryExists();
        }

        public async Task<FileInfo> SaveAsync(string filename, string content, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Nome do arquivo não pode estar vazio.", nameof(filename));

            if (content is null)
                throw new ArgumentNullException(nameof(content));

            var filePath = Path.Combine(this.diretorio, filename);
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await Task.Run(() => File.WriteAllText(filePath, content), cancellationToken);

            return new FileInfo(filePath);
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(this.diretorio))
                Directory.CreateDirectory(this.diretorio);
        }
    }
}
