using System.Xml;

namespace FiscalCore.Servicos.Utils
{
    public class SoapEnvelopes
    {
        public static XmlDocument FabricarEnvelopeCancelarNFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(envelope);
            return soapEnvelopeDocument;
        }

        public static XmlDocument FabricarEnvelopeInutilizacaoNFe(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">" +
            $"<soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeInutilizacao4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(envelope);
            return soapEnvelopeDocument;
        }

        public static XmlDocument FabricarEnvelopeAutorizacaoNFe4(string xml)
        {
            var envelope = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><nfeDadosMsg xmlns=\"http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4\">{xml}</nfeDadosMsg></soap12:Body></soap12:Envelope>";
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(envelope);
            return soapEnvelopeDocument;
        }
    }
}
