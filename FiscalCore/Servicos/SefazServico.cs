using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace FiscalCore.Servicos
{
    public class SefazServico
    {
        public async static Task<string> EnviarParaSefazAsync(IConfiguracaoServico cfgServico, UrlSefaz sefazUrl, XmlDocument envelope)
        {
            HttpWebRequest webRequest = SoapEnvelopeFabrica.CriarWebRequest(sefazUrl.Url, "application/soap+xml;charset=utf-8");

            Soap.InserirSoapEnvelopeWebRequest(envelope, webRequest);

            webRequest.ClientCertificates.Add(ObterCertificado.Obter(cfgServico.ConfigCertificado));

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = await rd.ReadToEndAsync();
                }
            }

            return soapResult;
        }

        public static UrlSefaz ObterUrl(eTipoServico tipoServico, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, eUF uf)
        {
            var urlAction = UrlsSefaz().Where(x => x.Servico == tipoServico && x.UF == uf && x.TipoAmbiente == tipoAmbiente && x.ModeloDocumento == modeloDocumento).FirstOrDefault();
            if (urlAction == null)
                throw new ArgumentOutOfRangeException();
            return urlAction;
        }

        public static IList<UrlSefaz> UrlsSefaz()
        {
            var urls = new List<UrlSefaz>();
            urls.Add(new UrlSefaz(eTipoServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx", "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new UrlSefaz(eTipoServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new UrlSefaz(eTipoServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new UrlSefaz(eTipoServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));

            urls.Add(new UrlSefaz(eTipoServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));

            urls.Add(new UrlSefaz(eTipoServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new UrlSefaz(eTipoServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new UrlSefaz(eTipoServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new UrlSefaz(eTipoServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));

            urls.Add(new UrlSefaz(eTipoServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new UrlSefaz(eTipoServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new UrlSefaz(eTipoServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new UrlSefaz(eTipoServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));

            urls.Add(new UrlSefaz(eTipoServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new UrlSefaz(eTipoServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));


            #region Ambiente Nacional
            // Distribuicao DFe - Consultar Documentos Destinados
            urls.Add(new UrlSefaz(eTipoServico.NFeDistribuicaoDFe, eUF.AN, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://www1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx", "https://www1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx?op=nfeDistDFeInteresse"));
            // Manifestação Destinatário
            urls.Add(new UrlSefaz(eTipoServico.ManifestacaoDestinatario, eUF.AN, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx", "https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?op=nfeRecepcaoEventoNF"));
            #endregion
            return urls;
        }
    }

    public class UrlSefaz
    {
        public string Url { get; private set; }
        public string Action { get; private set; }
        public eModeloDocumento ModeloDocumento { get; private set; }
        public eTipoAmbiente TipoAmbiente { get; private set; }
        public eUF UF { get; private set; }
        public eTipoServico Servico { get; set; }

        public UrlSefaz(eTipoServico servico, eUF uf, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, string url, string action)
        {
            Url = url;
            Action = action;
            UF = uf;
            TipoAmbiente = tipoAmbiente;
            ModeloDocumento = modeloDocumento;
            Servico = servico;
        }
    }
}
