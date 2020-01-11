using FiscalCore.Main.Configuracoes;
using FiscalCore.Main.Enums;
using FiscalCore.Main.Utils;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace FiscalCore.Servicos.Utils
{
    internal class Sefaz
    {
        [Obsolete("Parametro Modelo Documento não é necessário")]
        public static string EnviarParaSefaz(ConfiguracaoServico cfgServico, eModeloDocumento modeloDocumento, SefazUrl sefazUrl, XmlDocument envelope)
        {
            return EnviarParaSefaz(cfgServico, sefazUrl, envelope);
        }

        public static string EnviarParaSefaz(ConfiguracaoServico cfgServico, SefazUrl sefazUrl, XmlDocument envelope)
        {
            HttpWebRequest webRequest = Soap.CreateWebRequest(sefazUrl.Url, sefazUrl.Action, "application/soap+xml;charset=utf-8");

            Soap.InsertSoapEnvelopeIntoWebRequest(envelope, webRequest);

            webRequest.ClientCertificates.Add(Certificado.GetCertificado(cfgServico.Certificado.Serial));

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            return soapResult;
        }
    }
}
