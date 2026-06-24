using System;
using System.IO;
using System.Xml;

namespace FiscalCore.Utils
{
    public class Soap
    {
        private static string ObterTag(string ret, string tag)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(new StringReader(ret));
            }
            catch (XmlException ex)
            {
                throw new InvalidOperationException(
                    $"Resposta SEFAZ não é XML válido (tag '{tag}' não localizável). Conteúdo: {Resumir(ret)}", ex);
            }

            XmlNodeList xmlList = doc.GetElementsByTagName(tag);
            if (xmlList.Count == 0 || xmlList[0] is null)
                throw new InvalidOperationException(
                    $"Tag '{tag}' não encontrada na resposta SEFAZ. Conteúdo: {Resumir(ret)}");

            return xmlList[0]!.OuterXml;
        }

        private static string Resumir(string conteudo)
        {
            if (string.IsNullOrEmpty(conteudo))
                return "<vazio>";

            return conteudo.Length <= 500 ? conteudo : conteudo.Substring(0, 500) + "...";
        }

        public static XmlElement LimparEnvelope(string soapResult, string tag)
        {
            var xmlTag = Soap.ObterTag(soapResult, tag);

            var documento = new XmlDocument();
            documento.LoadXml(xmlTag);

            return documento.DocumentElement!;
        }
    }
}
