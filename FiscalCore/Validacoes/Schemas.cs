using FiscalCore.Tipos;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Validacoes
{
    public static class Schemas
    {
        public static string ObterSchema(eTipoServico tipoServico)
        {
            var schema = ListaSchemas.Where(x => x.TipoServico == tipoServico).SingleOrDefault();
            return schema.Arquivo;
        }

        private static IList<Schema> ListaSchemas => new List<Schema>
        {
            //OPERAÇAO EM LOTE
            // new Schema { TipoServico = eTipoServico.AutorizarNFe , Arquivo = "enviNFe_v4.00.xsd" },

            new Schema { TipoServico = eTipoServico.AutorizarNFe , Arquivo = "nfe_v4.00.xsd" },
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
