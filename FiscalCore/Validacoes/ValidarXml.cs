using FiscalCore.Configuracoes;
using FiscalCore.Tipos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace FiscalCore.Validacoes
{
    public class ValidarXml
    {
        private readonly string arquivoSchema;
        public IList<string> Erros = new List<string>();
        public bool Valido => Erros.Count == 0;

        public ValidarXml(eTipoServico tipoServico, ConfiguracaoBasicaServico cfgServico)
        {
            var caminhoSchema = cfgServico.DiretorioSchemas;
            arquivoSchema = PathArquivoSchema(caminhoSchema, tipoServico);
            ConferirSchemas(arquivoSchema);
        }

        public ValidarXml(eTipoServico tipoServico, ConfiguracaoServico cfgServico)
        {
            var caminhoSchema = cfgServico.DiretorioSchemas;

            arquivoSchema = PathArquivoSchema(caminhoSchema, tipoServico);

            ConferirSchemas(arquivoSchema);
        }

        private string PathArquivoSchema(string diretorioSchemas, eTipoServico tipoServico)
        {
            if (!Directory.Exists(diretorioSchemas))
                throw new FileNotFoundException("Diretório de Schemas não encontrado: " + diretorioSchemas);

            var arquivoSchema = Schemas.ObterSchema(tipoServico);
            return Path.Combine(diretorioSchemas, arquivoSchema);
        }

        private void ConferirSchemas(string arquivoSchema)
        {
            if (!File.Exists(arquivoSchema))
                throw new FileNotFoundException($"Arquivo não encontrado: {arquivoSchema}");
        }

        public void Validar(string xml)
        {
            var cfg = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            cfg.Schemas.XmlResolver = new XmlUrlResolver();
            cfg.Schemas.Add(null, arquivoSchema);

            cfg.ValidationEventHandler += delegate (object sender, ValidationEventArgs args)
            {
                Erros.Add($"[{args.Severity}] - {args.Message} {args.Exception?.Message} na linha {args.Exception.LineNumber} posição {args.Exception.LinePosition} em {args.Exception.SourceUri}".ToString());
            };

            var reader = XmlReader.Create(new StringReader(xml), cfg);
            var document = new XmlDocument();
            document.Load(reader);

            try
            {
                while (reader.Read()) { }
            }
            catch { }
            finally 
            {
                reader.Close();
            }
        }

        public override string ToString()
        {
            var result = string.Join(Environment.NewLine, Erros);
            return result;
        }
    }
}
