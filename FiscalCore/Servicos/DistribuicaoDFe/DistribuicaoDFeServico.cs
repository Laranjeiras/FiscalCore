using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe
{
    public class DistribuicaoDFeServico : BaseSefazServicoBasico<DistribuicaoDFeServico>
    {
        public DistribuicaoDFeServico(
                ConfiguracaoBasicaServico configuracao, 
                ITransmitirSefazCommand transmitir, 
                ILogger<DistribuicaoDFeServico> logger,
                IStorageContext storageContext
            ) 
            : base(configuracao, transmitir, logger, storageContext)
        {
        }

        public async Task<retDistDFeInt> ConsultarPorUltimoNSUAsync(string ultimoNsu, CancellationToken cancellation)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR ÚLTIMO NSU [{ultimoNsu}]");

            ultimoNsu = TratarNSU(ultimoNsu);

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.CNPJEmitente,
                DistNSU = new distNSU
                {
                    UltNSU = ultimoNsu
                },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, cancellation);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | ÚLTIMO NSU [{ultimoNsu}]");

            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorNSUAsync(string nsu, CancellationToken cancellation)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR NSU [{nsu}]");

            nsu = TratarNSU(nsu);

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.CNPJEmitente,
                consNSU = new consNSU { NSU = nsu },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, cancellation);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | NSU [{nsu}]");

            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorChaveAsync(ChaveFiscal chaveNFe, CancellationToken cancellation)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR CHAVE [{chaveNFe.Chave}]");

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.CNPJEmitente,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe.Chave
                },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, cancellation);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | CHAVE [{chaveNFe}]");

            return retDistDFeInt;
        }

        private static string TratarNSU(string nsu)
        {
            if (string.IsNullOrEmpty(nsu))
            {
                throw new ArgumentNullException(nameof(nsu));
            }
                
            if (nsu.Length > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(nsu));
            }

            nsu = nsu.PadLeft(15, '0');
            return nsu;
        }

        private async Task<retDistDFeInt> PrepararETransmitir(distDFeInt distDFeInt, CancellationToken cancellation)
        {
            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (configuracao.ValidarXmlSchema)
            {
                ValidarXml(eTipoServico.NFeDistribuicaoDFe, configuracao, xml);
            }

            var arqEnv = Path.Combine("Logs", $"{configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml");
            await SalvarLog(arqEnv, xml, cancellation);

            logger?.LogDebug($"FABRICAR ENVELOPE");
            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);
            logger?.LogDebug($"ENVELOPE FABRICADO");

            logger?.LogInformation($"OBTER URL DA SEFAZ");
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.NFeDistribuicaoDFe, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            logger?.LogInformation($"URL OBTIDA {sefazUrl.Url}");

            var retorno = await transmitir.TransmitirAsync(sefazUrl, envelope);

            logger?.LogDebug($"LIMPAR ENVELOPE");
            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;
            logger?.LogDebug($"ENVELOPE LIMPO");

            var arqRet = Path.Combine("Logs", $"{configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-retDistDFeInt.xml");
            await SalvarLog(arqRet, retornoLimpo, cancellation);

            logger?.LogDebug($"DESERIALIZANDO XML");
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retornoLimpo);
            logger?.LogDebug($"XML DESERIALIZADO");

            return retDistDFeInt;
        }
    }
}
