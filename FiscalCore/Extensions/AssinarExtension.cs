using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Utils;
using System;
using System.Security.Cryptography.X509Certificates;

namespace FiscalCore.Extensions
{
    public static class AssinarExtension
    {
        public static NFe Assinar(this NFe nfe, X509Certificate2 certificadoDigital)
        {
            var assinatura = Assinador.ObterAssinatura<NFe>(nfe, nfe.infNFe.Id, certificadoDigital);
            nfe.Signature = assinatura.Parse();
            return nfe;
        }

        public static Modelos.Eventos.evento Assinar(this Modelos.Eventos.evento evento, X509Certificate2 certificadoDigital, string signatureMethodSignedXml, string digestMethodReference)
        {
            var eventoLocal = evento;
            if (eventoLocal.infEvento.Id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(eventoLocal, eventoLocal.infEvento.Id, certificadoDigital, signatureMethodSignedXml, digestMethodReference);
            eventoLocal.Signature = assinatura;
            return eventoLocal;
        }

        public static Modelos.Inutilizacao.inutNFe Assinar(this Modelos.Inutilizacao.inutNFe inutNFe, X509Certificate2 certificadoDigital)
        {
            var inutNFeLocal = inutNFe;
            if (inutNFeLocal.infInut.Id == null)
                throw new Exception("Não é possível assinar um onjeto inutNFe sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(inutNFeLocal, inutNFeLocal.infInut.Id, certificadoDigital);
            inutNFeLocal.Signature = assinatura;
            return inutNFeLocal;
        }
    }
}
