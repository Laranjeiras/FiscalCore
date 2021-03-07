using System.IO;
using System.Net;
using System.Xml;

namespace FiscalCore.Utils
{
    public class Soap
    {
        public static void InserirSoapEnvelopeWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.PreserveWhitespace = true;
                soapEnvelopeXml.Save(stream);
            }
        }

        private static string ObterTag(string ret, string tag)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(ret));
            XmlNodeList xmlList = doc.GetElementsByTagName(tag);
            var xmlConverter = xmlList[0].OuterXml;
            return xmlConverter;
        }

        public static XmlElement LimparEnvelope(string soapResult, string tag)
        {
            var xmlTag = Soap.ObterTag(soapResult, tag);

            var documento = new XmlDocument();
            documento.LoadXml(xmlTag);

            return documento.DocumentElement;
        }
    }
}
