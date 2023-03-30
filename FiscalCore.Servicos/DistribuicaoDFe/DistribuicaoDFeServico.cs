using AlgoPlus.Storage.Services;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FiscalCore.Servicos.DistribuicaoDFe
{
    public class DistribuicaoDFeServico : BaseSefazServicoBasico
    {
        private readonly IStorage storage;
        private readonly ILogger<DistribuicaoDFeServico> logger;

        public DistribuicaoDFeServico(ConfiguracaoBasicaServico configuracao, ITransmitirSefazCommand transmitir, IStorage storage = null, ILogger<DistribuicaoDFeServico> logger = null)
            : base(configuracao, transmitir)
        {
            this.storage = storage;
            this.logger = logger;
        }

        public async Task<retDistDFeInt> ConsultarPorUltimoNSUAsync(string ultimoNsu, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR ÚLTIMO NSU [{ultimoNsu}]");
            
            if (string.IsNullOrEmpty(ultimoNsu))
            {
                throw new ArgumentNullException(nameof(ultimoNsu));
            }
                
            if (ultimoNsu.Length > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(ultimoNsu));
            }

            ultimoNsu = ultimoNsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.CNPJEmitente,
                DistNSU = new distNSU
                {
                    UltNSU = ultimoNsu
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, validarXmlConsulta);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | ÚLTIMO NSU [{ultimoNsu}]");

            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorNSUAsync(string nsu, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR NSU [{nsu}]");
            
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));
            if (nsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(nsu));

            nsu = nsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.CNPJEmitente,
                consNSU = new consNSU { NSU = nsu },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, validarXmlConsulta);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | NSU [{nsu}]");

            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorChaveAsync(ChaveFiscal chaveNFe, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"CONSULTAR DOCUMENTOS DESTINADOS POR CHAVE [{chaveNFe.Chave}]");

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.CNPJEmitente,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe.Chave
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retDistDFeInt = await PrepararETransmitir(distDFeInt, validarXmlConsulta);

            logger?.LogInformation($"DOCUMENTOS CONSULTADO COM SUCESSO | CHAVE [{chaveNFe}]");

            return retDistDFeInt;
        }

        private async Task<retDistDFeInt> PrepararETransmitir(distDFeInt distDFeInt, bool validarXmlConsulta = true)
        {

            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            logger.LogDebug($"VALIDANDO XML");

            if (validarXmlConsulta)
            {
                var validacao = new ValidarXml(eTipoServico.NFeDistribuicaoDFe, Configuracao);
                validacao.Validar(xml);
                if (!validacao.Valido)
                {
                    throw new FalhaValidacaoException(validacao.ToString());
                }
            }
            logger.LogDebug($"XML VALIDADO");

            var arqEnv = Path.Combine("Logs", $"{Configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml");
            await SalvarLog(arqEnv, xml);

            logger.LogDebug($"FABRICAR ENVELOPE");
            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);
            logger.LogDebug($"ENVELOPE FABRICADO");

            logger.LogInformation($"OBTER URL DA SEFAZ");
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.NFeDistribuicaoDFe, Configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            logger.LogInformation($"URL OBTIDA {sefazUrl.Url}");

            var retorno = await Transmitir.TransmitirAsync(sefazUrl, envelope);

            logger.LogDebug($"LIMPAR ENVELOPE");
            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;
            logger.LogDebug($"ENVELOPE LIMPO");

            var arqRet = Path.Combine("Logs", $"{Configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-retDistDFeInt.xml");
            await storage.SaveAsync(arqRet, retornoLimpo);

            logger.LogDebug($"DESERIALIZANDO XML");
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retornoLimpo);
            logger.LogDebug($"XML DESERIALIZADO");

            return retDistDFeInt;
        }

        private async Task SalvarLog(string filename, string conteudo)
        {
            if (storage == null)
            {
                return;
            }

            logger.LogInformation($"SALVAR LOG XML {filename}");
            var fileInfo = await storage.SaveAsync(filename, conteudo);
            logger.LogInformation($"LOG SALVO {fileInfo.AbsolutePath}");
        }
    }
}
