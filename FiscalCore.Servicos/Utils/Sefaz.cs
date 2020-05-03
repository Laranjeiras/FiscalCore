using FiscalCore.Main.Configuracoes;
using FiscalCore.Main.Utils;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace FiscalCore.Servicos.Utils
{
    internal class Sefaz
    {
        public static string EnviarParaSefaz(IConfiguracaoServico cfgServico, SefazUrl sefazUrl, XmlDocument envelope)
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
