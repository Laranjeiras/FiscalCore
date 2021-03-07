using DFeBR.EmissorNFe.Utilidade.Entidades;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using Newtonsoft.Json;
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
        public static void ValidarSchema(eTipoServico tipoServico, string xml, IConfiguracaoServico cfgServico)
        {
            var caminhoSchema = cfgServico.DiretorioSchemas;

            if (!Directory.Exists(caminhoSchema))
                throw new Exception("Diretório de Schemas não encontrado: \n" + caminhoSchema);

            var cfg = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            var schemas = ObterSchema(tipoServico);

            foreach (var schema in schemas)
            {
                var arquivoSchema = Path.Combine(caminhoSchema, schema);
                if (!File.Exists(arquivoSchema))
                    throw new FileNotFoundException($"Arquivo não encontrado: {arquivoSchema}");
                cfg.Schemas.Add(null, arquivoSchema);
            }

            cfg.ValidationEventHandler += ValidationEventHandler;

            var reader = XmlReader.Create(new StringReader(xml), cfg);
            var document = new XmlDocument();
            document.Load(reader);

            //Valida xml
            document.Validate(ValidationEventHandler);
        }

        public static IList<string> ObterSchema(eTipoServico tiposervico)
        {
            var schema = ListaSchemas.Where(x => x.TipoServico == tiposervico).SingleOrDefault();
            return schema.Arquivos.ToList();
        }

        private static IList<Schema> ListaSchemas => new List<Schema>
        {
            new Schema {
                TipoServico = eTipoServico.AutorizarNFe ,
                Arquivos = new string[] { "enviNFe_v4.00.xsd", "leiauteNFe_v4.00.xsd", "tiposBasico_v4.00.xsd", "nfe_v4.00.xsd","xmldsig-core-schema_v1.01.xsd" }
            },
            new Schema
            {
                TipoServico = eTipoServico.NFeDistribuicaoDFe,
                Arquivos = new string[] { "distDFeInt_v1.01.xsd", "tiposDistDFe_v1.01.xsd" }
            }
        };

        private static void ValidationEventHandler(object sender, ValidationEventArgs ex)
        {
            var msg = $"Erro ao validar xml contra Schema Xsd.\n{ex.Message}";
            TraceException(ex.Exception, msg);
            throw new FalhaValidacaoSchemaException(msg);
        }

        public static void TraceException(Exception ex, string msg)
        {
            var str = new StringBuilder();
            var error = new ErrorTrace
            {
                Data = DateTime.Now.ToString("g"),
                Detalhe = ex.GetType().Name,
                Mensagem = $"{ex.Message}.{msg}",
                StackTrace = ex.StackTrace
            };
            var content = JsonConvert.SerializeObject(error);
            str.Append(content);
            str.Append('?');
            str.Append(Environment.NewLine);
            Console.WriteLine(str.ToString());
        }
    }

    public class Schema
    {
        public eTipoServico TipoServico { get; set; }

        public string[] Arquivos { get; set; }
    }
}
