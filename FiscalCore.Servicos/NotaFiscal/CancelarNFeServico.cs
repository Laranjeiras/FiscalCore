using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos
{
    public class CancelarNFeServico
    {
        private readonly ConfiguracaoServico _cfgServico;
        string _versao;

        public CancelarNFeServico(ConfiguracaoServico cfgServico)
        {
            _cfgServico = cfgServico;
            _versao = "1.00";
        }

        public async Task<retEnvEvento> Cancelar(IList<InfoNFeCancelar> infos) 
        {
            if (infos == null || infos.Count <= 0)
                throw new Exception("Informações da NFe não encontrada");

            if (infos.Count > 20)
                throw new Exception("No máximo 20 NFes podem ser canceladas");

            List<evento> eventos = new List<evento>();

            eModeloDocumento _modeloDocumento = infos[0].ChaveAcesso.Substring(20, 2).ModeloDocumento();

            foreach (var item in infos)
            {
                var protocolo = item.ProtocoloAutorizacao;
                var chave = item.ChaveAcesso;
                var just = item.Justificativa;

                if (string.IsNullOrEmpty(protocolo))
                    throw new Exception("Protocolo de autorização não informado");
                if(!(protocolo.Length == 15))
                    throw new Exception("Protocolo de autorização inválido");
                if (string.IsNullOrEmpty(chave))
                    throw new Exception("Chave da NFe não informada");
                if(string.IsNullOrEmpty(just) || just.Length < 15 || just.Length > 255)
                    throw new Exception("Justificativa de conter entre 15 e 255 caracteres");

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
                    tpEvento = eTipoEventoNFe.NFeCancelamento,
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
            return await Cancelar(eventos, _modeloDocumento);
        }

        private async Task<retEnvEvento> Cancelar(List<evento> eventos, eModeloDocumento modeloDoc) 
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

                var _certificado = ObterCertificado.Obter(_cfgServico.ConfigCertificado);
                eventoTmp.Assinar(_certificado, _cfgServico.ConfigCertificado.SignatureMethodSignedXml, _cfgServico.ConfigCertificado.DigestMethodReference);
            }

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            await Arquivo.SalvarArquivoAsync(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-eve.xml", xmlEvento);

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.CancelarNFe, _cfgServico.TipoAmbiente, modeloDoc, _cfgServico.UF);
            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.CancelarNFe, xmlEvento);

            var retornoXmlString = await SefazServico.EnviarParaSefazAsync(_cfgServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retEnvEvento").OuterXml;

            await Arquivo.SalvarArquivoAsync(_cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ret-eve.xml", retornoXmlStringLimpa);

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
