using FiscalCore.Exceptions;
using FiscalCore.ValueObjects;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace FiscalCore.Utils
{
    public static class Certificado
    {
        internal static X509Certificate2 ObterDoRepositorio(string serial)
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

        internal static X509Certificate2 ObterDeArquivo(string arquivo, string senha)
        {
            try
            {
                if (!File.Exists(arquivo))
                    throw new FileNotFoundException(string.Format("Certificado digital {0} não encontrado!", arquivo));
                var certificado = new X509Certificate2(arquivo, senha, X509KeyStorageFlags.MachineKeySet);
                return certificado;
            } 
            catch(System.Security.Cryptography.CryptographicException ex)
            {
                if (ex.Message.Contains("password is not correct"))
                {
                    throw new SenhaInvalidaException("Senha inválida", ex);
                }
                throw;
            }
            catch(Exception)
            {
                throw;
            }
        }

        internal static X509Certificate2 ObterDeArquivo(byte[] bytes, string senha)
        {
            return new X509Certificate2(bytes, senha, X509KeyStorageFlags.Exportable);
        }

        public static string? ExtrairCNPJArquivo(this X509Certificate2 certificado)
        {
            const string oIdSubjectAlternativeName = "2.5.29.17";

            var extensao = certificado.Extensions[oIdSubjectAlternativeName];
            var texto = Encoding.UTF8.GetString(extensao!.RawData);

            var matches = Regex.Matches(texto, @"(?<!\d)\d{14}(?!\d)");

            var cnpjValido = matches.FirstOrDefault(p => new Cnpj(p.Value).Valido);
            return cnpjValido?.Value;
        }

        public static DateTime Validade(this X509Certificate2 certificado) 
            => certificado.NotAfter;

        public static bool Expirado(this X509Certificate2 certificado) 
            => Validade(certificado) <= DateTime.Now;

        public static void Validar(this X509Certificate2 certificado)
        {
            if (certificado.Expirado())
            {
                throw new ConfiguracaoException($"CERTIFICADO EXPIRADO EM {certificado.Validade()}");
            }
        }
    }
}
