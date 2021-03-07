using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FiscalCore.Utils
{
    public class Xml
    {
        private static readonly Hashtable CacheSerializers = new Hashtable();

        public static string ClasseParaXmlString<T>(T objeto)
        {
            XElement xml;
            var ser = XmlSerializer.FromTypes(new[] { typeof(T) })[0];

            using (var memory = new MemoryStream())
            {
                using (TextReader tr = new StreamReader(memory, Encoding.UTF8))
                {
                    ser.Serialize(memory, objeto);
                    memory.Position = 0;
                    xml = XElement.Load(tr);
                    xml.Attributes().Where(x => x.Name.LocalName.Equals("xsd") || x.Name.LocalName.Equals("xsi")).Remove();
                }
            }
            return XElement.Parse(xml.ToString()).ToString(SaveOptions.DisableFormatting);
        }

        public static T XmlStringParaClasse<T>(string input) where T : class
        {
            var ser = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            using var sr = new StringReader(input);
            return (T)ser.Deserialize(sr);
        }

        public static string ObterTagXml(string xml, string tag)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xml));
            XmlNodeList xmlList = doc.GetElementsByTagName(tag);
            var xmlConverter = xmlList[0].OuterXml;
            return xmlConverter;
        }

        public static string SalvarArquivoXml(string dir, string nomeArquivo, string xmlString)
        {
            if (!Directory.Exists(dir))
                throw new Exception("Diretorio não existe");
            var filename = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(filename);
            stw.WriteLine(xmlString);
            stw.Close();
            return filename;
        }

        public static async Task<string> SalvarArquivoXmlAsync(string dir, string nomeArquivo, string xmlString)
        {
            if (!Directory.Exists(dir))
                throw new Exception("Diretorio não existe");
            var filename = Path.Combine(dir, nomeArquivo);
            var stw = new StreamWriter(filename);
            await stw.WriteLineAsync(xmlString);
            stw.Close();
            return filename;
        }
    }
}
