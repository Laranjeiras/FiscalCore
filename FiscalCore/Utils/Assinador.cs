using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Signature = FiscalCore.Modelos.Signatures.Signature;

namespace FiscalCore.Utils
{
    public static class Assinador
    {
        public static Signature ObterAssinatura<T>(T objeto, string id, X509Certificate2 certificadoDigital,
            string signatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            string digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1") where T : class
        {

            if (id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var documento = new XmlDocument { PreserveWhitespace = true };

            var xml = XmlUtils.ClasseParaXmlString(objeto);
            documento.LoadXml(xml);

            AsymmetricAlgorithm privateKey = certificadoDigital.GetRSAPrivateKey()
                ?? (AsymmetricAlgorithm?)certificadoDigital.GetECDsaPrivateKey()
                ?? throw new InvalidOperationException("Não foi possível obter a chave privada do certificado.");

            var docXml = new SignedXml(documento) { SigningKey = privateKey };

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
            var assinatura = XmlUtils.XmlStringParaClasse<Signature>(xmlDigitalSignature.OuterXml);
            return assinatura;
        }
    }
}
