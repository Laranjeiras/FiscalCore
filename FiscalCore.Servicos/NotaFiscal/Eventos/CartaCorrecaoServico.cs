using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class CartaCorrecaoServico : IEventoServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly string _versao;

        public CartaCorrecaoServico(ConfiguracaoServico cfgServico)
        {
            this.cfgServico = cfgServico;
            _versao = "1.00";
        }

        public async Task<retEnvEvento> TransmitirCorrecao(IList<InfoCartaCorrecao> infos) 
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
                eModeloDocumento _modeloDocumento = chave.Substring(20, 2).ModeloDocumento();

                if (_modeloDocumento != eModeloDocumento.NFe)
                    throw new Exception("Somente NFe podem ter Carta Correção");

                var infEvento = new infEventoEnv
                {
                    chNFe = chave,
                    CNPJ = cfgServico.Emitente.CNPJ,
                    CPF = cfgServico.Emitente.CPF,
                    cOrgao = cfgServico.UF,
                    dhEvento = DateTime.Now,
                    nSeqEvento = nSeqEvento,
                    tpAmb = cfgServico.TipoAmbiente,
                    tpEvento = eTipoEventoNFe.NFeCartaCorrecao,
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

                var _certificado = ObterCertificado.Obter(cfgServico.ConfigCertificado);
                eventoTmp.Assinar(_certificado, cfgServico.ConfigCertificado.SignatureMethodSignedXml, cfgServico.ConfigCertificado.DigestMethodReference);
            }

            var pedEvento = new envEvento
            {
                versao = _versao,
                idLote = 1,
                evento = eventos
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            await Arquivo.SalvarArquivoAsync(cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-eve.xml", xmlEvento);

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.CartaCorrecao, cfgServico.TipoAmbiente, eModeloDocumento.NFe, cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CartaCorrecao, xmlEvento);

            var retornoXmlString = await SefazServico.EnviarParaSefazAsync(cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            await Arquivo.SalvarArquivoAsync(cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ret-eve.xml", retornoXmlStringLimpa);

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
