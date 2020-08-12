using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using KeyInfo = System.Security.Cryptography.Xml.KeyInfo;
using Reference = System.Security.Cryptography.Xml.Reference;
using Signature = FiscalCore.Modelos.Signatures.Signature;

namespace FiscalCore.Utils
{
    public class Assinador
    {
        public static Signature ObterAssinatura<T>(T objeto, string id, X509Certificate2 certificadoDigital,
            bool manterDadosEmCache = false, string signatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            string digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1", bool cfgServicoRemoverAcentos = false) where T : class
        {
            var objetoLocal = objeto;
            if (id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            try
            {
                var documento = new XmlDocument { PreserveWhitespace = true };

                documento.LoadXml(FuncoesXml.ClasseParaXmlString(objetoLocal));

                var docXml = new SignedXml(documento) { SigningKey = certificadoDigital.PrivateKey };

                docXml.SignedInfo.SignatureMethod = signatureMethod;

                var reference = new Reference { Uri = "#" + id, DigestMethod = digestMethod };

                var envelopedSigntature = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(envelopedSigntature);

                var c14Transform = new XmlDsigC14NTransform();
                reference.AddTransform(c14Transform);

                docXml.AddReference(reference);

                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificadoDigital));

                docXml.KeyInfo = keyInfo;
                docXml.ComputeSignature();

                var xmlDigitalSignature = docXml.GetXml();
                var assinatura = FuncoesXml.XmlStringParaClasse<Signature>(xmlDigitalSignature.OuterXml);
                return assinatura;
            }
            finally
            {
                //Marcos Gerene 04/08/2018 - o objeto certificadoDigital nunca será nulo, porque se ele for nulo nem as configs para criar ele teria.

                //Se não mantém os dados do certificado em cache libera o certificado, chamando o método reset.
                //if (!manterDadosEmCache & certificadoDigital == null)
                //     certificadoDigital.Reset();
            }

        }
    }
}
