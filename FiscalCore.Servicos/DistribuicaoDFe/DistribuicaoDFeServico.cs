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
        private readonly ILogger logger;

        public DistribuicaoDFeServico(ConfiguracaoBasicaServico configuracao, ITransmitirSefazCommand transmitir, IStorage storage = null, ILogger logger = null)
            : base(configuracao, transmitir)
        {
            this.storage = storage;
            this.logger = logger;
        }

        public async Task<retDistDFeInt> ConsultarPorUltimoNSUAsync(string ultimoNsu, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"Consultar documentos destinados, ultimo NSU {ultimoNsu}");
            
            if (string.IsNullOrEmpty(ultimoNsu))
                throw new ArgumentNullException(nameof(ultimoNsu));
            if (ultimoNsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(ultimoNsu));

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
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorNSUAsync(string nsu, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"Consultar documentos destinados por NSU {nsu}");
            
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
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorChaveAsync(ChaveFiscal chaveNFe, bool validarXmlConsulta = true)
        {
            logger?.LogInformation($"Consultar documentos destinados por Chave {chaveNFe.Chave}");

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
            return retDistDFeInt;
        }

        private async Task<retDistDFeInt> PrepararETransmitir(distDFeInt distDFeInt, bool validarXmlConsulta = true)
        {
            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (validarXmlConsulta)
            {
                var validacao = new ValidarXml(eTipoServico.NFeDistribuicaoDFe, Configuracao);
                validacao.Validar(xml);
                if (!validacao.Valido)
                    throw new FalhaValidacaoException(validacao.ToString());
            }

            if (storage != null)
            {
                var nomeArqEnv = $"{Configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml";
                var arqEnv = Path.Combine("Logs", nomeArqEnv);
                await storage.SaveAsync(arqEnv, xml);
            }

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);

            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.NFeDistribuicaoDFe, Configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var retorno = await Transmitir.TransmitirAsync(sefazUrl, envelope);

            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;

            if (storage != null)
            {
                var nomeArqRetorno = $"{Configuracao.CNPJEmitente}-{DateTime.Now.Ticks}-retDistDFeInt.xml";
                var arqRet = Path.Combine("Logs", nomeArqRetorno);
                await storage.SaveAsync(arqRet, retornoLimpo);
            }

            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retornoLimpo);

            return retDistDFeInt;
        }
    }
}
