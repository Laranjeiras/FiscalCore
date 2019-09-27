using FiscalCore.Main.Models.Eventos;
using FiscalCore.Main.Models.Retornos;
using FiscalCore.Main.Utils;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Main.Extensions
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

        public static string ObterXmlString(this retEnvEvento retEnvEvento)
        {
            return FuncoesXml.ClasseParaXmlString<retEnvEvento>(retEnvEvento);
        }

        public static retEnvEvento CarregarDeXmlString(this retEnvEvento retEnvEvento, string xmlString)
        {
            return FuncoesXml.XmlStringParaClasse<retEnvEvento>(xmlString);
        }

        public static evento Assina(this evento evento, X509Certificate2 certificadoDigital, string signatureMethodSignedXml, string digestMethodReference)
        {
            var eventoLocal = evento;
            if (eventoLocal.infEvento.Id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(eventoLocal, eventoLocal.infEvento.Id, certificadoDigital, false, signatureMethodSignedXml, digestMethodReference);
            eventoLocal.Signature = assinatura;
            return eventoLocal;
        }
    }
}
