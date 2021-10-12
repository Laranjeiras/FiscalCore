using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using KeyInfo = System.Security.Cryptography.Xml.KeyInfo;
using Reference = System.Security.Cryptography.Xml.Reference;
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
            var assinatura = XmlUtils.XmlStringParaClasse<Signature>(xmlDigitalSignature.OuterXml);
            return assinatura;
        }

        public static DFeBR.EmissorNFe.Dominio.Assinatura.Signature Parse(this Modelos.Signatures.Signature origem)
        {
            return new DFeBR.EmissorNFe.Dominio.Assinatura.Signature
            {
                KeyInfo = new DFeBR.EmissorNFe.Dominio.Assinatura.KeyInfo
                {
                    X509Data = new DFeBR.EmissorNFe.Dominio.Assinatura.X509Data
                    {
                        X509Certificate = origem.KeyInfo.X509Data.X509Certificate
                    }
                },
                SignatureValue = origem.SignatureValue,
                SignedInfo = new DFeBR.EmissorNFe.Dominio.Assinatura.SignedInfo
                {
                    CanonicalizationMethod = new DFeBR.EmissorNFe.Dominio.Assinatura.CanonicalizationMethod
                    {
                        Algorithm = origem.SignedInfo.CanonicalizationMethod.Algorithm
                    },
                    Reference = new DFeBR.EmissorNFe.Dominio.Assinatura.Reference
                    {
                        DigestMethod = new DFeBR.EmissorNFe.Dominio.Assinatura.DigestMethod
                        {
                            Algorithm = origem.SignedInfo.Reference.DigestMethod.Algorithm
                        },
                        DigestValue = origem.SignedInfo.Reference.DigestValue,
                        Transforms = origem.SignedInfo.Reference.Transforms.Select(x =>
                            new DFeBR.EmissorNFe.Dominio.Assinatura.Transform
                            {
                                Algorithm = x.Algorithm
                            }).ToList(),
                        URI = origem.SignedInfo.Reference.URI
                    },
                    SignatureMethod = new DFeBR.EmissorNFe.Dominio.Assinatura.SignatureMethod
                    {
                        Algorithm = origem.SignedInfo.SignatureMethod.Algorithm
                    }
                }
            };
        }

    }
}
