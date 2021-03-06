using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Enums;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.Autorizacao;
using FiscalCore.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace FiscalCore.Extensions
{
    public static class NFeExtension
    {
        public static NFe Assinar(this NFe nfe, X509Certificate2 certificadoDigital)
        {
            var assinatura = Assinador.ObterAssinatura<NFe>(nfe, nfe.infNFe.Id, certificadoDigital);
            nfe.Signature = assinatura.Parse();
            return nfe;
        }

        public static string String(this NFe nfe)
        {
            return FuncoesXml.ClasseParaXmlString<NFe>(nfe);
        }

        public static string String(this nfeProc nfeProc)
        {
            return FuncoesXml.ClasseParaXmlString<nfeProc>(nfeProc);
        }

        public static string String(this enviNFe enviNFe)
        {
            return FuncoesXml.ClasseParaXmlString<enviNFe>(enviNFe);
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

        public static eModeloDocumento Parse(this DFeBR.EmissorNFe.Utilidade.Tipos.ModeloDocumento origem)
        {
            return (eModeloDocumento) (int)origem;
        }
    }
}
