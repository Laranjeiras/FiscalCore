using System;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Main.Utils
{
    public class Certificado
    {
        public static X509Certificate2 GetCertificado(string serial)
        {
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = store.Certificates;
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySerialNumber, serial, false);
                if (signingCert.Count == 0)
                    return null;
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
