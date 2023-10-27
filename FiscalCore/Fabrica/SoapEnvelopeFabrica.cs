using FiscalCore.Tipos;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FiscalCore.Fabrica
{
    public static class SoapEnvelopeFabrica
    {
        public static XmlDocument FabricarEnvelope(eTipoServico tipoServico, string xml)
        {
            switch (tipoServico)
            {
                case eTipoServico.AutorizarNFe:
                    return FabricarEnvelopeAutorizacaoNFe4(xml);
                case eTipoServico.CancelarNFe:
                    return FabricarEnvelopeEventoNFe(xml);
                case eTipoServico.InutilizacaoNFe:
                    return FabricarEnvelopeInutilizacaoNFe(xml);
                case eTipoServico.ConsultaSituacaoNFe:
                    return FabricarEnvelopeConsultarSituacaoNFe(xml);
                case eTipoServico.CartaCorrecao:
                    return FabricarEnvelopeEventoNFe(xml);
                case eTipoServico.ManifestacaoDestinatario:
                    return FabricarEnvelopeManifestacaoNacional(xml);
                case eTipoServico.NFeDistribuicaoDFe:
                    return FabricarEnvelopeNFeDistribuicaoDFe(xml);
                default:
                    return null;
            }
        }

        private static XmlDocument FabricarEnvelopeEventoNFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument FabricarEnvelopeInutilizacaoNFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeInutilizacao4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument FabricarEnvelopeAutorizacaoNFe4(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument FabricarEnvelopeConsultarSituacaoNFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
                $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeConsultaProtocolo4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument FabricarEnvelopeNFeDistribuicaoDFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDistDFeInteresse xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe\"><nfeDadosMsg>{xml}</nfeDadosMsg></nfeDistDFeInteresse></soap12:Body></soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument FabricarEnvelopeManifestacaoNacional(string xml)
        {
            var envelope = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
                "<soap12:Body>" +
                    "<nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4\">" +
                        $"{xml}" +
                    "</nfeDadosMsg>" +
                "</soap12:Body>" +
                "</soap12:Envelope>";
            return CriarEnvelopeSoap(envelope);
        }

        private static XmlDocument CriarEnvelopeSoap(string request)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(request);
            return soapEnvelopeDocument;
        }

        public static HttpWebRequest CriarWebRequest(string url, X509Certificate2 certificado)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "application/soap+xml;charset=utf-8";
            webRequest.Method = "POST";
            webRequest.ClientCertificates.Add(certificado);

            return webRequest;
        }
    }
}
