using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Utils
{
    public static class ObterCertificado
    {
        //public DateTime Validade => Convert.ToDateTime(ObterCertificado().GetExpirationDateString());
        //public string IssuedTo => ObterCertificado().Issuer;
        //public string FriendlyName => ObterCertificado().FriendlyName;

        public static X509Certificate2 Obter(ConfiguracaoCertificado configCertificado)
        {
            if (string.IsNullOrEmpty(configCertificado.Serial))
                throw new Exception("Serial do certificado não informado");

            switch (configCertificado.TipoCertificado)
            {
                case TipoCertificado.A1Repositorio:
                    return ObterDoRepositorio(configCertificado.Serial);
                case TipoCertificado.A1Arquivo:
                    return ObterDeArquivo(configCertificado.ArquivoCertificado, configCertificado.Senha);
                case TipoCertificado.A3:
                    return ObterDoRepositorio(configCertificado.Serial);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static X509Certificate2 ObterDoRepositorio(string serial)
        {
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
    }
}
