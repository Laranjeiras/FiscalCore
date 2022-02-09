using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Servicos.NotaFiscal.Eventos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.NotaFiscal
{
    public class NotaFiscalServico : BaseSefazServico
    {
        public NotaFiscalServico(ConfiguracaoServico configuracao, IStorage storage, ITransmitirSefazCommand transmitir) : base(configuracao, transmitir)
        {
            if (configuracao.VersaoAutorizacaoNFe == eVersaoServico.Versao400)
                autorizarNFe = new AutorizarNFe4(configuracao, transmitir);
            else
                throw new NotImplementedException("Versão de autorização da NFe não suportada");
            this.storage = storage;
        }

        private readonly IStorage storage;
        private readonly IAutorizarNFeServico autorizarNFe;
        public IAutorizarNFeServico AutorizarNFe => autorizarNFe;
        

        private CancelarNFeServico cancelarNFe;
        public CancelarNFeServico CancelarNFe
        {
            get
            {
                if (cancelarNFe == null)
                    cancelarNFe = new CancelarNFeServico(Configuracao, storage, Transmitir);
                return cancelarNFe;
            }
        }

        private ComprovanteEntregaNFeServico eventoComprovanteEntregaNFe;
        public ComprovanteEntregaNFeServico EventoComprovateEntregaNFe
        {
            get
            {
                if (eventoComprovanteEntregaNFe == null)
                    eventoComprovanteEntregaNFe = new ComprovanteEntregaNFeServico(Configuracao, Transmitir);
                return eventoComprovanteEntregaNFe;
            }
        }

        public async Task<retConsSitNFe> ConsultarPelaChave(string chaveAcesso, string versao)
        {
            var chave = new ChaveFiscal(chaveAcesso);
            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso,
                tpAmb = Configuracao.TipoAmbiente,
                versao = versao
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-pedConsSitNFe.xml");
            await storage.SaveAsync(arqEnv, xmlEvento);

            var modeloDoc = chave.Modelo;

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.ConsultaSituacaoNFe, Configuracao.TipoAmbiente, modeloDoc, Configuracao.UF);

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await Transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-retConsSitNFe.xml");
            await storage.SaveAsync(arqRet, retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }

        private CartaCorrecaoServico cartaCorrecaoServico;
        public CartaCorrecaoServico CartaCorrecaoServico
        {
            get
            {
                if (cartaCorrecaoServico == null)
                    cartaCorrecaoServico = new CartaCorrecaoServico(Configuracao, storage, Transmitir);
                return cartaCorrecaoServico;
            }
        }

        private InutilizarNFeServico inutilizarNFeServico;
        
        public InutilizarNFeServico InutilizarNFeServico
        {
            get
            {
                if (inutilizarNFeServico == null)
                    inutilizarNFeServico = new InutilizarNFeServico(Configuracao, Transmitir);
                return inutilizarNFeServico;
            }
        }
    }
}
