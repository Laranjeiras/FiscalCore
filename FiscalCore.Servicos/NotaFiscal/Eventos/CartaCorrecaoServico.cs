using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using System.IO;
using AlgoPlus.Storage.Services;
using FiscalCore.Exceptions;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class CartaCorrecaoServico : IEventoServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand transmitir;
        private readonly string _versao;

        public CartaCorrecaoServico(ConfiguracaoServico cfgServico, IStorage storage, ITransmitirSefazCommand transmitir)
        {
            this.cfgServico = cfgServico;
            this.storage = storage;
            this.transmitir = transmitir;
            _versao = "1.00";
        }

        public async Task<retEnvEvento> TransmitirCorrecao(InfoCartaCorrecao info)
        {
            return await TransmitirCorrecao(new List<InfoCartaCorrecao> { info });
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

                if (item.Correcao.Length < 15 || item.Correcao.Length > 1000)
                    throw new FalhaValidacaoException("A descrição da correção deve conter entre 15 e 1000 caracteres");

                if (_modeloDocumento != eModeloDocumento.NFe)
                    throw new FalhaValidacaoException("Somente NFe pode ter Carta Correção");

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

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-eve.xml", cfgServico));
            await storage.SaveAsync(arqEnv, xmlEvento);

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.CartaCorrecao, cfgServico.TipoAmbiente, eModeloDocumento.NFe, cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CartaCorrecao, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-eve.xml", cfgServico));
            await storage.SaveAsync(arqRet, retornoXmlStringLimpa);

             var retorno = new retEnvEvento().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retorno;
        }
    }
}
