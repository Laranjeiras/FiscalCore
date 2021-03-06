using FiscalCore.Utils;

namespace FiscalCore.Modelos.Retornos
{
    public class RetNFeAutorizacao4 : BaseRetorno, IRetornoAutorizacao
    {
        public RetNFeAutorizacao4(string xmlRetorno) : base(xmlRetorno)
        {
            Retorno = FuncoesXml.XmlStringParaClasse<retEnviNFe>(XmlRecebido);
        }

        public retEnviNFe Retorno { get; set; }
    }
}
