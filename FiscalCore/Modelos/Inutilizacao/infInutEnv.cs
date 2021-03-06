using FiscalCore.Enums;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Inutilizacao
{
    public class infInutEnv
    {
        public infInutEnv()
        {
        }

        [XmlAttribute]
        public string Id { get; set; }

        public eTipoAmbiente tpAmb { get; set; }

        public string xServ { get; set; } = "INUTILIZAR";

        public eUF cUF { get; set; }

        public int ano { get; set; }

        public string CNPJ { get; set; }

        public eModeloDocumento mod { get; set; }

        public int serie { get; set; }

        public int nNFIni { get; set; }

        public int nNFFin { get; set; }

        public string xJust { get; set; }
    }
}