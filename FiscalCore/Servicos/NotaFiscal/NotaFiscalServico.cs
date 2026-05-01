using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.NotaFiscal
{
    public class NotaFiscalServico : BaseSefazServico<NotaFiscalServico>
    {
        public NotaFiscalServico(ConfiguracaoServico configuracao, IAutorizarNFeServico nfeServico, IStorageContext storageContext, ITransmitirSefazCommand transmitir, ILogger<NotaFiscalServico> logger) 
            : base(configuracao, transmitir, logger, storageContext)
        {
            this.AutorizarNFe = nfeServico;
        }

        public IAutorizarNFeServico AutorizarNFe { get; private set; }

        public async Task<retConsSitNFe> ConsultarPelaChave(string chaveAcesso, string versao, CancellationToken cancellation)
        {
            var chave = new ChaveFiscal(chaveAcesso);
            var consSit = new consSitNFe
            {
                chNFe = chaveAcesso,
                tpAmb = configuracao.TipoAmbiente,
                versao = versao
            };

            var xmlEvento = XmlUtils.ClasseParaXmlString<consSitNFe>(consSit);

            var arqEnv = Path.Combine("Logs", $"{DateTime.Now.Ticks}-pedConsSitNFe.xml");
            
            await SalvarLog(arqEnv, xmlEvento, cancellation);

            var modeloDoc = chave.Modelo;

            var sefazUrl = Fabrica.FabricarUrl.ObterUrl(eTipoServico.ConsultaSituacaoNFe, configuracao.TipoAmbiente, modeloDoc, configuracao.UF);

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ConsultaSituacaoNFe, xmlEvento);

            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope!);

            var retornoXmlStringLimpa = Soap.LimparEnvelope(retornoXmlString, "retConsSitNFe").OuterXml;

            var arqRet = Path.Combine("Logs", $"{DateTime.Now.Ticks}-retConsSitNFe.xml");

            await SalvarLog(arqRet, retornoXmlStringLimpa, cancellation);

            var retEnvEvento = new retConsSitNFe().CarregarDeXmlString(retornoXmlStringLimpa, xmlEvento);

            return retEnvEvento;
        }
    }
}
