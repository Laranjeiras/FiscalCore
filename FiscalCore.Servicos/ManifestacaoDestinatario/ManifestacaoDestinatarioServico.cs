using FiscalCore.Configuracoes;
using FiscalCore.DTOs.ManifestacaoDestinatario;
using FiscalCore.Extensions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.ManifestacaoDestinatario
{
    public class ManifestacaoDestinatarioServico
    {
        private readonly ConfiguracaoServico configuracao;
        private readonly string versao;
        private readonly int nSeqEvento;
        private readonly X509Certificate2 certificado;

        public ManifestacaoDestinatarioServico(ConfiguracaoServico configuracao)
        {
            this.configuracao = configuracao;
            this.versao = "1.00";
            this.nSeqEvento = 1;
            this.certificado = ObterCertificado.Obter(configuracao.ConfigCertificado);
        }

        public async Task<string> ManifestarAsync(string chaveAcesso, eTipoEventoNFe tipoEvento, string justificativa = null)
        {
            var xmlEvento = GerarXmlEvento(chaveAcesso, tipoEvento, justificativa);

            await Arquivo.SalvarArquivoAsync(configuracao, "Eventos", $"{DateTime.Now.Ticks}-ped-eve.xml", xmlEvento);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
            var sefazUrl = SefazServico.ObterUrl(eTipoServico.ManifestacaoDestinatario, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var xmlRetorno = await SefazServico.EnviarParaSefazAsync(configuracao, sefazUrl, envelope);
            var xmlRetLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;
            await Arquivo.SalvarArquivoAsync(configuracao, "Eventos", $"{DateTime.Now.Ticks}-ret-eve.xml", xmlRetLimpo);
            return xmlRetLimpo;
        }

        private retEnvEvento MontarRetorno(string xml)
        {
            return XmlUtils.XmlStringParaClasse<retEnvEvento>(xml);
        }

        public RetEventoManifestacaoDTO MontarDTO(string xml)
        {
            var retEnv = MontarRetorno(xml);
            if (retEnv.retEvento.Count != 1)
                throw new Exception("Retorno com mais de 1 evento registrado");

            var inf = retEnv.retEvento.SingleOrDefault();
            return new RetEventoManifestacaoDTO(inf.infEvento);
        }

        private string GerarXmlEvento(string chaveAcesso, eTipoEventoNFe tipoEvento, string justificativa)
        {
            if (tipoEvento != eTipoEventoNFe.OperacaoNaoRealizada)
                justificativa = null;
            else
                throw new ArgumentNullException(nameof(justificativa));

            var infEvento = new infEventoEnv
            {
                chNFe = chaveAcesso,
                CNPJ = configuracao.Emitente.CNPJ,
                CPF = configuracao.Emitente.CPF,
                cOrgao = eUF.AN,
                dhEvento = DateTime.Now,
                nSeqEvento = nSeqEvento,
                tpAmb = configuracao.TipoAmbiente,
                tpEvento = tipoEvento,
                verEvento = versao,
                Id = "ID" + ((int)tipoEvento) + chaveAcesso + nSeqEvento.ToString().PadLeft(2, '0'),
                detEvento = new detEvento()
                {
                    versao = versao,
                    descEvento = tipoEvento.Descricao().RemoverAcentos()
                }
            };

            var evento = new evento {
                versao = versao,
                infEvento = infEvento
            };

            evento.Assinar(certificado, configuracao.ConfigCertificado.SignatureMethodSignedXml, configuracao.ConfigCertificado.DigestMethodReference);

            var pedEvento = new envEvento
            {
                versao = versao,
                idLote = 1,
                evento = new List<evento> { evento }
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);

            return xmlEvento;
        }
    }
}
