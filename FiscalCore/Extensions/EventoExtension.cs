using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;

namespace FiscalCore.Extensions
{
    public static class EventoExtension
    {

        public static retEnvEvento CarregarDeXmlString(this retEnvEvento retEnvEvento, string xmlRecebido, string xmlEnviado)
        {
            var tmp = Xml.XmlStringParaClasse<retEnvEvento>(xmlRecebido);
            tmp.XmlEnviado = xmlEnviado;
            tmp.XmlRecebido = xmlRecebido;
            return tmp;
        }

        public static retConsSitNFe CarregarDeXmlString(this retConsSitNFe retConsSitNFe, string xmlRecebido, string xmlEnviado)
        {
            var tmp = Xml.XmlStringParaClasse<retConsSitNFe>(xmlRecebido);
            tmp.XmlRecebido = xmlRecebido;
            tmp.XmlEnviado = xmlEnviado;
            return tmp;
        }
    }
}
