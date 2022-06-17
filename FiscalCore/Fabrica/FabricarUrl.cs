using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Fabrica
{
    public class FabricarUrl
    {
        public static UrlSefaz ObterUrl(eTipoServico tipoServico, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, eUF uf)
        {
            var urlAction = UrlsSefaz()
                .Where(x => 
                    x.Servico == tipoServico &&
                    x.UF == uf &&
                    x.TipoAmbiente == tipoAmbiente &&
                    x.ModeloDocumento == modeloDocumento)
                .FirstOrDefault();

            if (urlAction == null)
                throw new ArgumentOutOfRangeException("Nenhuma URL do Webservice encontrada");
            return urlAction;
        }

        private static IList<UrlSefaz> UrlsSefaz()
        {
            var urls = new List<UrlSefaz>();
            urls.Add(new UrlSefaz(eTipoServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));
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
            urls.Add(new UrlSefaz(eTipoServico.NFeDistribuicaoDFe, eUF.AN, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://hom1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx", "https://hom1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx?op=nfeDistDFeInteresse"));
            // Manifestação Destinatário
            urls.Add(new UrlSefaz(eTipoServico.ManifestacaoDestinatario, eUF.AN, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx", "https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?op=nfeRecepcaoEventoNF"));
            urls.Add(new UrlSefaz(eTipoServico.ManifestacaoDestinatario, eUF.AN, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://hom1.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx", "https://hom1.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?op=nfeRecepcaoEventoNF"));
            #endregion
            return urls;
        }

        public static UrlConsultaNfce ObterUrlConsultaNfce(eTipoAmbiente tipoAmbiente, eUF uf, eVersaoQrCode versaoQrCode)
        {
            try
            {
                var url = UrlQrCodeNfce
                    .Where(x =>
                        x.TipoAmbiente == tipoAmbiente &&
                        x.UF == uf &&
                        x.VersaoQrCode == versaoQrCode)
                    .SingleOrDefault();
                return url;
            } 
            catch
            {
                throw new Exception($"Não foi possível obter a url da consulta nfce para a UF {uf}");
            }
        }


        private static List<UrlConsultaNfce> UrlQrCodeNfce = new List<UrlConsultaNfce>
        {
            new UrlConsultaNfce(
                eTipoAmbiente.Homologacao,
                eUF.RJ,
                eVersaoQrCode.QrCodeVersao2,
                "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode",
                "www.fazenda.rj.gov.br/nfce/consulta"
            ),
            new UrlConsultaNfce(
                eTipoAmbiente.Producao,
                eUF.RJ,
                eVersaoQrCode.QrCodeVersao2,
                "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode",
                "www.fazenda.rj.gov.br/nfce/consulta"
            ),
        };
    }
}
