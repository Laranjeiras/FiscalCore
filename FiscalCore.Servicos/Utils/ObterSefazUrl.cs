using FiscalCore.Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Servicos.Utils
{
    internal class ObterSefazUrl
    {
        public static SefazUrl ObterUrl(fcServico tipoServico, eTipoAmbiente tipoAmbiente, eModeloDocumento modeloDocumento, eUF uf)
        {
            var urlAction = AutorizacaoUrls().Where(x => x.Servico == tipoServico && x.UF == uf && x.TipoAmbiente == tipoAmbiente && x.ModeloDocumento == modeloDocumento).FirstOrDefault();
            if (urlAction == null)
                throw new ArgumentOutOfRangeException();
            return urlAction;
        }

        public static IList<SefazUrl> AutorizacaoUrls()
        {
            var urls = new List<SefazUrl>();
            urls.Add(new SefazUrl(fcServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx", "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new SefazUrl(fcServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new SefazUrl(fcServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));
            urls.Add(new SefazUrl(fcServico.AutorizarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?op=nfeAutorizacaoLote"));

            urls.Add(new SefazUrl(fcServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CancelarNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));

            urls.Add(new SefazUrl(fcServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new SefazUrl(fcServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new SefazUrl(fcServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));
            urls.Add(new SefazUrl(fcServico.InutilizacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx", "https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?op=nfeInutilizacaoNF"));

            urls.Add(new SefazUrl(fcServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new SefazUrl(fcServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new SefazUrl(fcServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));
            urls.Add(new SefazUrl(fcServico.ConsultaSituacaoNFe, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx", "https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?op=nfeConsultaNF"));

            urls.Add(new SefazUrl(fcServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFe, "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFe, "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Producao, eModeloDocumento.NFCe, "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));
            urls.Add(new SefazUrl(fcServico.CartaCorrecao, eUF.RJ, eTipoAmbiente.Homologacao, eModeloDocumento.NFCe, "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx", "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?op=nfeRecepcaoEvento"));

            return urls;
        }
    }
}
