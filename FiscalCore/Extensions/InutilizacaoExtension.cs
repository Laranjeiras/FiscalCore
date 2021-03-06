using FiscalCore.Modelos.Inutilizacao;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Extensions
{
    public static class InutilizacaoExtension
    {
        public static string ObterXmlString(this inutNFe pedInutilizacao)
        {
            return FuncoesXml.ClasseParaXmlString(pedInutilizacao);
        }

        public static inutNFe Assina(this inutNFe inutNFe, X509Certificate2 certificadoDigital)
        {
            var inutNFeLocal = inutNFe;
            if (inutNFeLocal.infInut.Id == null)
                throw new Exception("Não é possível assinar um onjeto inutNFe sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(inutNFeLocal, inutNFeLocal.infInut.Id, certificadoDigital);
            inutNFeLocal.Signature = assinatura;
            return inutNFeLocal;
        }

        public static retInutNFe CarregarDeXmlString(this retInutNFe retInutNFe, string xmlString)
        {
            return FuncoesXml.XmlStringParaClasse<retInutNFe>(xmlString);
        }
        
        public static string ObterXmlString(this retInutNFe retInutNFe)
        {
            return FuncoesXml.ClasseParaXmlString(retInutNFe);
        }
    }
}
