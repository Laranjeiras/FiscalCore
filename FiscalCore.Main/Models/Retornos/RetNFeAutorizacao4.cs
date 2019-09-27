using FiscalCore.Main.Utils;

namespace FiscalCore.Main.Models.Retornos
{
    public class RetNFeAutorizacao4 : BaseRetorno
    {
        public RetNFeAutorizacao4(string xmlRetString) : base(xmlRetString)
        {
            Retorno = FuncoesXml.XmlStringParaClasse<retEnviNFe>(XmlRetStr);
        }

        public retEnviNFe Retorno { get; set; }
    }
}
