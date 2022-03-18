using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace FiscalCore.Validacoes
{
    public static class Schemas
    {
        public static void ValidarSchema(eTipoServico tipoServico, string xml, ConfiguracaoServico cfgServico)
        {
            var caminhoSchema = cfgServico.DiretorioSchemas;
            var erros = new List<string>();

            if (!Directory.Exists(caminhoSchema))
                throw new Exception("Diretório de Schemas não encontrado: " + caminhoSchema);

            var arquivoSchema = Path.Combine(caminhoSchema, ObterSchema(tipoServico));

            if (!File.Exists(arquivoSchema))
                throw new FileNotFoundException($"Arquivo não encontrado: {arquivoSchema}");

            var cfg = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            cfg.Schemas.XmlResolver = new XmlUrlResolver();
            cfg.Schemas.Add(null, arquivoSchema);

            cfg.ValidationEventHandler += delegate (object sender, ValidationEventArgs args)
            {
                erros.Add($"[{args.Severity}] {args.Message}");
                //Console.WriteLine($"[{args.Severity}] - {args.Message} {args.Exception?.Message} na linha {args.Exception.LineNumber} posição {args.Exception.LinePosition} em {args.Exception.SourceUri}".ToString()));
            };

            var reader = XmlReader.Create(new StringReader(xml), cfg);
            var document = new XmlDocument();
            document.Load(reader);

            if (erros.Count > 0)
            {
                var result = string.Join(Environment.NewLine, erros);
                throw new FalhaValidacaoException($"Erro ao validar XML contra Schema Xsd: {Environment.NewLine}{result}");
            }
        }

        public static string ObterSchema(eTipoServico tipoServico)
        {
            var schema = ListaSchemas.Where(x => x.TipoServico == tipoServico).SingleOrDefault();
            return schema.Arquivo;
        }

        private static IList<Schema> ListaSchemas => new List<Schema>
        {
            new Schema { TipoServico = eTipoServico.AutorizarNFe , Arquivo = "enviNFe_v4.00.xsd" },
            new Schema { TipoServico = eTipoServico.NFeDistribuicaoDFe, Arquivo = "distDFeInt_v1.01.xsd"         },
            new Schema { TipoServico = eTipoServico.ManifestacaoDestinatario, Arquivo = "envConfRecebto_v1.00.xsd" }
        };
    }

    public class Schema
    {
        public eTipoServico TipoServico { get; set; }

        public string Arquivo { get; set; }
    }
}
