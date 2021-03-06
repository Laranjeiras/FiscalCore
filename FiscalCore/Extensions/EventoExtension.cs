using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Extensions
{
    public static class EventoExtension
    {
        public static string ObterXmlString(this evento pedEvento)
        {
            return FuncoesXml.ClasseParaXmlString(pedEvento);
        }

        public static string ObterXmlString(this envEvento pedEvento)
        {
            return FuncoesXml.ClasseParaXmlString(pedEvento);
        }

        public static string ObterXmlString(this infEventoRet infEventoRet) 
        {
            return FuncoesXml.ClasseParaXmlString<infEventoRet>(infEventoRet);
        }

        public static string ObterXmlString(this retEnvEvento retEnvEvento)
        {
            return FuncoesXml.ClasseParaXmlString<retEnvEvento>(retEnvEvento);
        }

        public static retEnvEvento CarregarDeXmlString(this retEnvEvento retEnvEvento, string xmlRecebido, string xmlEnviado)
        {
            var tmp = FuncoesXml.XmlStringParaClasse<retEnvEvento>(xmlRecebido);
            tmp.XmlEnviado = xmlEnviado;
            tmp.XmlRecebido = xmlRecebido;
            return tmp;
        }

        public static string ObterXmlString(this consSitNFe consSiteNFe)
        {
            return FuncoesXml.ClasseParaXmlString(consSiteNFe);
        }

        public static retConsSitNFe CarregarDeXmlString(this retConsSitNFe retConsSitNFe, string xmlRecebido, string xmlEnviado)
        {
            var tmp = FuncoesXml.XmlStringParaClasse<retConsSitNFe>(xmlRecebido);
            tmp.XmlRecebido = xmlRecebido;
            tmp.XmlEnviado = xmlEnviado;
            return tmp;
        }

        public static evento Assina(this evento evento, X509Certificate2 certificadoDigital, string signatureMethodSignedXml, string digestMethodReference)
        {
            var eventoLocal = evento;
            if (eventoLocal.infEvento.Id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(eventoLocal, eventoLocal.infEvento.Id, certificadoDigital, signatureMethodSignedXml, digestMethodReference);
            eventoLocal.Signature = assinatura;
            return eventoLocal;
        }
    }
}
