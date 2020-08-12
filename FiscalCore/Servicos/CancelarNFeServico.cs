using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;
using System.Collections.Generic;

namespace FiscalCore.Servicos
{
    public class CancelarNFeServico
    {
        private readonly IConfiguracaoServico _cfgServico;
        string _versao;

        public CancelarNFeServico(IConfiguracaoServico cfgServico)
        {
            _cfgServico = cfgServico;
            _versao = "1.00";
        }

        public retEnvEvento Cancelar(IList<InfoNFeCancelar> infos) 
        {
            if (infos == null || infos.Count <= 0)
                throw new Exception("Informações da NFe não encontrada");

            if (infos.Count > 20)
                throw new Exception("No máximo 20 NFes podem ser canceladas");

            List<evento> eventos = new List<evento>();

            eModeloDocumento _modeloDocumento = Conversor.ModeloDocumento(infos[0].ChaveAcesso.Substring(20, 2));

            foreach (var item in infos)
            {
                var protocolo = item.ProtocoloAutorizacao;
                var chave = item.ChaveAcesso;
                var just = item.Justificativa;

                Zion.Common2.Assertions.ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(protocolo, "Protocolo de autorização não informado");
                Zion.Common2.Assertions.ZionAssertion.StringHasLen(protocolo, 15, 15, "Protocolo de autorização inválido");
                Zion.Common2.Assertions.ZionAssertion.StringIsNullOrEmptyOrWhiteSpace(chave, "Chave da NFe não informada");
                Zion.Common2.Assertions.ZionAssertion.StringHasLen(just, 15, 255, "Justificativa de conter entre 15 e 255 caracteres");

                var detEvento = new detEvento
                {
                    nProt = protocolo,
                    versao = _versao,
                    xJust = just,
                    descEvento = "Cancelamento",
                };

                var infEvento = new infEventoEnv
                {
                    cOrgao = _cfgServico.UF,
                    tpAmb = _cfgServico.TipoAmbiente,
                    chNFe = chave,
                    dhEvento = DateTime.Now,
                    tpEvento = eNFeTipoEvento.NfeCancelamento,
                    nSeqEvento = 1,
                    verEvento = _versao,
                    detEvento = detEvento
                };

                if (!string.IsNullOrEmpty(_cfgServico.Emitente.CNPJ))
                    infEvento.CNPJ = _cfgServico.Emitente.CNPJ;
                else
                    infEvento.CPF = _cfgServico.Emitente.CPF;

                var evento = new evento { versao = _versao, infEvento = infEvento };
                eventos.Add(evento);

            }            
            return Cancelar(eventos, _modeloDocumento);
        }

        private retEnvEvento Cancelar(List<evento> eventos, eModeloDocumento modeloDoc) 
        {
            var pedEvento = new envEvento
            {
                versao = _versao,
                idLote = 1,
                evento = eventos
            };

            foreach (var eventoTmp in eventos)
            {
                eventoTmp.infEvento.Id = "ID" + ((int)eventoTmp.infEvento.tpEvento) + eventoTmp.infEvento.chNFe +
                                      eventoTmp.infEvento.nSeqEvento.ToString().PadLeft(2, '0');

                var _certificado = Certificado.GetCertificado(_cfgServico.Certificado.Serial);
                eventoTmp.Assina(_certificado, _cfgServico.Certificado.SignatureMethodSignedXml, _cfgServico.Certificado.DigestMethodReference);
            }

            var xmlEvento = pedEvento.ObterXmlString();

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-eve.xml", xmlEvento);

            var sefazUrl = ObterSefazUrl.ObterUrl(fcServico.CancelarNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = SoapEnvelopes.FabricarEnvelopeEventoNFe(xmlEvento);

            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.ClearEnvelop(retornoXmlString, "retEnvEvento").OuterXml;

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ret-eve.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;

        }
    }

    public class InfoNFeCancelar
    {
        public string ChaveAcesso { get; private set; }
        public string ProtocoloAutorizacao { get; private set; }
        public string Justificativa { get; private set; }

        public InfoNFeCancelar(string chaveAcesso, string protocoloAutorizacao, string justificativa = "Nota Fiscal Emitida Indevidamente")
        {
            ChaveAcesso = chaveAcesso.Replace("NFe", "");
            ProtocoloAutorizacao = protocoloAutorizacao;
            Justificativa = justificativa;
        }
    }

}
