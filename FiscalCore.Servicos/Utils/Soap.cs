using System.IO;
using System.Net;
using System.Xml;

namespace FiscalCore.Servicos.Utils
{
    public class Soap
    {
        public static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.PreserveWhitespace = true;
                soapEnvelopeXml.Save(stream);
            }
        }

        public static XmlDocument CreateSoapEnvelope(string request)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(request);
            return soapEnvelopeDocument;
        }

        public static HttpWebRequest CreateWebRequest(string url, string action, string contentType)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = contentType;
            webRequest.Method = "POST";
            return webRequest;
        }

        private static string GetTagConverter(string ret, string tag)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(ret));
            XmlNodeList xmlList = doc.GetElementsByTagName(tag);
            var xmlConverter = xmlList[0].OuterXml;
            return xmlConverter;
        }

        public static XmlElement ClearEnvelop(string soapResult, string tag)
        {
            var xmlTag = Soap.GetTagConverter(soapResult, tag);

            var documento = new XmlDocument();
            documento.LoadXml(xmlTag);

            return documento.DocumentElement;
        }
    }
}
