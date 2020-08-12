using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FiscalCore.Utils
{
    public class FuncoesXml
    {
        private static readonly Hashtable CacheSerializers = new Hashtable();

        public static string ClasseParaXmlString<T>(T objeto)
        {
            XElement xml;
            var keyNomeClasseEmUso = typeof(T).FullName;
            var ser = BuscarNoCache(keyNomeClasseEmUso, typeof(T));

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
            var keyNomeClasseEmUso = typeof(T).FullName;
            var ser = BuscarNoCache(keyNomeClasseEmUso, typeof(T));

            using (var sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }

        private static XmlSerializer BuscarNoCache(string chave, Type type)
        {
            if (CacheSerializers.Contains(chave))
            {
                return (XmlSerializer)CacheSerializers[chave];
            }

            var ser = XmlSerializer.FromTypes(new[] { type })[0];
            CacheSerializers.Add(chave, ser);

            return ser;
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
    }
}
