using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiscalCore.Servicos
{
    public class CartaCorrecaoServico
    {
        private readonly IConfiguracaoServico _cfgServico;
        private readonly string _versao;

        public CartaCorrecaoServico(IConfiguracaoServico cfgServico)
        {
            _cfgServico = cfgServico;
            _versao = "1.00";
        }

        public retEnvEvento TransmitirCorrecao(IList<InfoCartaCorrecao> infos) 
        {
            if (infos == null || infos.Count <= 0)
                throw new Exception("Informações da NFe não encontrada");

            if (infos.Count > 20)
                throw new Exception("No máximo 20 NFes podem ser Corrigidas");


            List<evento> eventos = new List<evento>();

            foreach (var item in infos)
            {
                var chave = item.ChaveAcesso;
                var correcao = item.Correcao;
                var nSeqEvento = item.nSeqEvento;
                chave = chave.Replace("NFe", "");
                eModeloDocumento _modeloDocumento = Conversor.ModeloDocumento(chave.Substring(20, 2));

                if (_modeloDocumento != eModeloDocumento.NFe)
                    throw new Exception("Somente NFe podem ter Carta Correção");

                var infEvento = new infEventoEnv
                {
                    chNFe = chave,
                    CNPJ = _cfgServico.Emitente.CNPJ,
                    CPF = _cfgServico.Emitente.CPF,
                    cOrgao = _cfgServico.UF,
                    dhEvento = DateTime.Now,
                    nSeqEvento = nSeqEvento,
                    tpAmb = _cfgServico.TipoAmbiente,
                    tpEvento = eNFeTipoEvento.NfeCartaCorrecao,
                    verEvento = _versao,
                    detEvento = new detEvento()
                    {
                        versao = _versao,
                        descEvento = "Carta de Correção",
                        xCorrecao = correcao
                    }
                };
                var evento = new evento { versao = _versao, infEvento = infEvento };
                eventos.Add(evento);
            }

            foreach (var eventoTmp in eventos)
            {
                eventoTmp.infEvento.Id = "ID" + ((int)eventoTmp.infEvento.tpEvento) + eventoTmp.infEvento.chNFe +
                                      eventoTmp.infEvento.nSeqEvento.ToString().PadLeft(2, '0');

                var _certificado = ObterCertificado.ObterCertificado(_cfgServico.ConfigCertificado.Serial);
                eventoTmp.Assina(_certificado, _cfgServico.ConfigCertificado.SignatureMethodSignedXml, _cfgServico.ConfigCertificado.DigestMethodReference);
            }

            var pedEvento = new envEvento
            {
                versao = _versao,
                idLote = 1,
                evento = eventos
            };

            var xmlEvento = pedEvento.ObterXmlString();

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-eve.xml", xmlEvento);

            var sefazUrl = ObterSefazUrl.ObterUrl(TipoServico.CartaCorrecao, _cfgServico.TipoAmbiente, eModeloDocumento.NFe, _cfgServico.UF);
            var envelope = SoapEnvelopes.FabricarEnvelopeEventoNFe(xmlEvento);

            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.ClearEnvelop(retornoXmlString, "retEnvEvento").OuterXml;

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ret-eve.xml", retornoXmlStringLimpa);

            var retorno = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retorno;

        }
    }

    public class InfoCartaCorrecao 
    {
        public InfoCartaCorrecao(string chaveAcesso, string correcao, int nSequenciaEvento)
        {
            ChaveAcesso = chaveAcesso;
            Correcao = correcao;
            nSeqEvento = nSequenciaEvento;
        }

        public string ChaveAcesso { get; private set; }

        public string Correcao { get; private set; }

        public int nSeqEvento { get; private set; }
    }
}
