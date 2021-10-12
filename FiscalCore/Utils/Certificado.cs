using FiscalCore.Configuracoes;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace FiscalCore.Utils
{
    public static class ObterCertificado
    {
        public static X509Certificate2 Obter(ConfiguracaoCertificado configCertificado)
        {
            switch (configCertificado.TipoCertificado)
            {
                case eTipoCertificado.A1Repositorio:
                    return ObterDoRepositorio(configCertificado.Serial);
                case eTipoCertificado.A1Arquivo:
                    return ObterDeArquivo(configCertificado.ArquivoCertificado, configCertificado.Senha);
                case eTipoCertificado.A3:
                    return ObterDoRepositorio(configCertificado.Serial);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static X509Certificate2 ObterDoRepositorio(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                throw new Exception("Serial do certificado não informado");

            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection currentCerts = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySerialNumber, serial, false);

                if (signingCert.Count == 0)
                    throw new Exception(string.Format("Certificado digital nº {0} não encontrado!", serial.ToUpper()));

                var certificado = signingCert[0];
                return certificado;
            }
            finally
            {
                store.Close();
            }
        }

        private static X509Certificate2 ObterDeArquivo(string arquivo, string senha)
        {
            if (!File.Exists(arquivo))
                throw new FileNotFoundException(string.Format("Certificado digital {0} não encontrado!", arquivo));
            var certificado = new X509Certificate2(arquivo, senha, X509KeyStorageFlags.MachineKeySet);
            return certificado;
        }

        public static string ExtrairCNPJArquivo(X509Certificate2 certificado)
        {
            const string oIdSubjectAlternativeName = "2.5.29.17";

            var extensao = certificado.Extensions[oIdSubjectAlternativeName];
            var texto = Encoding.UTF8.GetString(extensao.RawData);

            var matches = Regex.Matches(texto, @"(?<!\d)\d{14}(?!\d)");

            var cnpjValido = matches.FirstOrDefault(p => new Cnpj(p.Value).EhValido);
            return cnpjValido?.Value;
        }

        public static DateTime Validade(this X509Certificate2 certificado)
        {
            return certificado.NotAfter;
        }
    }
}
