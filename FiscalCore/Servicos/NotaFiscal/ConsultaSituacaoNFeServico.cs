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
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FiscalCore.Servicos
{
    public class ConsultaSituacaoNFeServico : BaseSefazServicoBasico<ConsultaSituacaoNFeServico>
    {
        private readonly ITransmitirSefazCommand sefaz;
        #pragma warning disable CS0414
        private readonly CancellationToken cancellation;
        #pragma warning restore CS0414
        private const string versao= "4.00";

        public ConsultaSituacaoNFeServico(ConfiguracaoBasicaServico cfgServico, IStorageContext storage, ITransmitirSefazCommand transmitir, ILogger<ConsultaSituacaoNFeServico> logger)
            :base(cfgServico, transmitir, logger, storage)
        {
            this.sefaz = transmitir;
            this.cancellation = new CancellationToken();
        }

        public async Task<retConsSitNFe> ConsultarPelaChave(ChaveFiscal chaveAcesso, CancellationToken cancellation) 
        {
            //chaveAcesso = chaveAcesso.Replace("NFe", "");

            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso.Chave,
                tpAmb = configuracao.TipoAmbiente,
                versao = versao
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("pedConsSitNFe.xml", configuracao));
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            var modeloDoc = chaveAcesso.Modelo;

            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ConsultaSituacaoNFe, configuracao.TipoAmbiente, modeloDoc, configuracao.UF);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope!);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("retConsSitNFe.xml", configuracao));
            await SalvarLog(arqRet, retornoXmlStringLimpa, cancellation);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}
