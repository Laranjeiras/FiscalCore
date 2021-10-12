using FiscalCore.Utils;

namespace FiscalCore.Modelos.Retornos
{
    public class RetNFeAutorizacao4 : BaseRetorno, IRetornoAutorizacao
    {
        public RetNFeAutorizacao4(string xmlRetorno) : base(xmlRetorno)
        {
            Retorno = XmlUtils.XmlStringParaClasse<retEnviNFe>(XmlRecebido);
        }

        public retEnviNFe Retorno { get; set; }

        public bool Autorizado => Retorno?.cStat == 100 ? true : false;
    }
}
