using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using FiscalCore.Fabrica;
using AlgoPlus.Storage.Services;
using System.IO;
using FiscalCore.ValueObjects;

namespace FiscalCore.Servicos
{
    public class ConsultaSituacaoNFeServico
    {
        private readonly ConfiguracaoServico cfgServico;
        private readonly IStorage storage;
        private readonly ITransmitirSefazCommand sefaz;
        private readonly string versao;

        public ConsultaSituacaoNFeServico(ConfiguracaoServico cfgServico, IStorage storage, ITransmitirSefazCommand transmitir)
        {
            this.cfgServico = cfgServico;
            this.storage = storage;
            this.sefaz = transmitir;
            this.versao = "4.00";
        }

        public async Task<retConsSitNFe> ConsultarPelaChave(ChaveFiscal chave) 
        {
            var consSit = new consSitNFe
            {
                chNFe = chave.Chave,
                tpAmb = cfgServico.TipoAmbiente,
                versao = versao
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("pedConsSitNFe.xml", cfgServico));
            await storage.SaveAsync(arqEnv, xmlEvento);

            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ConsultaSituacaoNFe, cfgServico.TipoAmbiente, chave.Modelo, cfgServico.UF);
            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await sefaz.TransmitirAsync(sefazUrl, envelope);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("retConsSitNFe.xml", cfgServico));
            await storage.SaveAsync(arqRet, retornoXmlStringLimpa);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}
