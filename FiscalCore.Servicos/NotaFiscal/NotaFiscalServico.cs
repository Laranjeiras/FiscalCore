using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Servicos.NotaFiscal.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using System;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.NotaFiscal
{
    public class NotaFiscalServico
    {
        private readonly ConfiguracaoServico configuracaoServico;

        public NotaFiscalServico(ConfiguracaoServico configuracaoServico)
        {
            this.configuracaoServico = configuracaoServico;

            if (configuracaoServico.VersaoAutorizacaoNFe == eVersaoServico.Versao400)
                autorizarNFe = new AutorizarNFe4(configuracaoServico);
            else
                throw new NotImplementedException("Versão de autorização da NFe não suportada");
        }

        private IAutorizarNFeServico autorizarNFe;
        public IAutorizarNFeServico AutorizarNFe => autorizarNFe;

        private CancelarNFeServico cancelarNFe;
        public CancelarNFeServico CancelarNFe
        {
            get
            {
                if (cancelarNFe == null)
                    cancelarNFe = new CancelarNFeServico(configuracaoServico);
                return cancelarNFe;
            }
        }

        private ComprovanteEntregaNFeServico eventoComprovanteEntregaNFe;
        public ComprovanteEntregaNFeServico EventoComprovateEntregaNFe
        {
            get
            {
                if (eventoComprovanteEntregaNFe == null)
                    eventoComprovanteEntregaNFe = new ComprovanteEntregaNFeServico(configuracaoServico);
                return eventoComprovanteEntregaNFe;
            }
        }

        public async Task<retConsSitNFe> ConsultarPelaChave(string chaveAcesso, string versao)
        {
            //chaveAcesso = chaveAcesso.Replace("NFe", "");
            var chave = new ChaveFiscal(chaveAcesso);
            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso,
                tpAmb = configuracaoServico.TipoAmbiente,
                versao = versao
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            //var modeloDoc = chaveAcesso.Substring(20, 2).ModeloDocumento();
            var modeloDoc = chave.Modelo;

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.ConsultaSituacaoNFe, configuracaoServico.TipoAmbiente, modeloDoc, configuracaoServico.UF);

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await SefazServico.EnviarParaSefazAsync(configuracaoServico, sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            await Arquivo.SalvarArquivoAsync(configuracaoServico, DateTime.Now.Ticks + "-retConsSitNFe.xml", retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }

        private CartaCorrecaoServico cartaCorrecaoServico;
        public CartaCorrecaoServico CartaCorrecaoServico
        {
            get
            {
                if (cartaCorrecaoServico == null)
                    cartaCorrecaoServico = new CartaCorrecaoServico(configuracaoServico);
                return cartaCorrecaoServico;
            }
        }

        private InutilizarNFeServico inutilizarNFeServico;
        public InutilizarNFeServico InutilizarNFeServico
        {
            get
            {
                if (inutilizarNFeServico == null)
                    inutilizarNFeServico = new InutilizarNFeServico(configuracaoServico);
                return inutilizarNFeServico;
            }
        }
    }
}
