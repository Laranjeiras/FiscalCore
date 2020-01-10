using FiscalCore.Main.Configuracoes;
using FiscalCore.Main.Enums;
using FiscalCore.Main.Extensions;
using FiscalCore.Main.Models.Eventos;
using FiscalCore.Main.Models.Retornos;
using FiscalCore.Main.Utils;
using FiscalCore.Servicos.Utils;
using System;
using System.Collections.Generic;

namespace FiscalCore.Servicos.Servicos
{
    public class CancelarNFe
    {
        ConfiguracaoServico cfgServico;
        eModeloDocumento modeloDocumento;
        string versao;
        eTipoAutor tipoAutor;

        public CancelarNFe(ConfiguracaoServico cfgServico, string versao, eModeloDocumento modeloDocumento, eTipoAutor tipoAutor = eTipoAutor.EmpresaEmitente)
        {
            this.cfgServico = cfgServico;
            this.versao = versao;
            this.modeloDocumento = modeloDocumento;
            this.tipoAutor = tipoAutor;
        }

        public retEnvEvento Cancelar(string protocoloAutorizacao, string justificativa, string chaveNFe, string cpfCnpj)
        {
            chaveNFe = chaveNFe.Replace("NFe", "");
            var detEvento = new detEvento
            {
                nProt = protocoloAutorizacao,
                versao = versao,
                xJust = justificativa,
                descEvento = "Cancelamento",
            };

            var infEvento = new infEventoEnv
            {
                cOrgao = cfgServico.UF,
                tpAmb = cfgServico.TipoAmbiente,
                chNFe = chaveNFe,
                dhEvento = DateTime.Now,
                tpEvento = eNFeTipoEvento.NfeCancelamento,
                nSeqEvento = 1,
                verEvento = versao,
                detEvento = detEvento
            };

            if (cpfCnpj.Length == 11)
                infEvento.CPF = cpfCnpj;
            else
                infEvento.CNPJ = cpfCnpj;

            var evento = new evento { versao = versao, infEvento = infEvento };
            var eventos = new List<evento> { evento };

            var pedEvento = new envEvento
            {
                versao = versao,
                idLote = 1,
                evento = eventos
            };

            foreach (var eventoTmp in eventos)
            {
                eventoTmp.infEvento.Id = "ID" + ((int)eventoTmp.infEvento.tpEvento) + eventoTmp.infEvento.chNFe +
                                      eventoTmp.infEvento.nSeqEvento.ToString().PadLeft(2, '0');

                var _certificado = Certificado.GetCertificado(cfgServico.Certificado.Serial);
                eventoTmp.Assina(_certificado, cfgServico.Certificado.SignatureMethodSignedXml, cfgServico.Certificado.DigestMethodReference);
            }

            var xmlEvento = pedEvento.ObterXmlString();

            FuncoesXml.SalvarArquivoXml(cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-eve.xml", xmlEvento);

            var sefazUrl = ObterSefazUrl.ObterUrl(fcServico.CancelarNFe, cfgServico.TipoAmbiente, modeloDocumento, cfgServico.UF);
            var envelope = SoapEnvelopes.FabricarEnvelopeCancelarNFe(xmlEvento);

            var retornoXmlString = Sefaz.EnviarParaSefaz(cfgServico, modeloDocumento, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.ClearEnvelop(retornoXmlString, "retEnvEvento").OuterXml;

            FuncoesXml.SalvarArquivoXml(cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ret-eve.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);
            
            return retEnvEvento;
        }
    }
}
