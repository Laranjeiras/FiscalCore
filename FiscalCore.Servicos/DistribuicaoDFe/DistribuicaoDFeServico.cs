using FiscalCore.Configuracoes;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Servicos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FiscalCore.DistribuicaoDFe.Servicos
{
    public class DistribuicaoDFeServico : BaseSefazServico
    {
        private readonly ILogger logger;

        public DistribuicaoDFeServico(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir, ILogger<DistribuicaoDFeServico> logger)
            : base(configuracao, transmitir)
        {
            this.logger = logger;
        }

        public async Task<retDistDFeInt> ConsultarDocumentosDestinadosAsync(string ultimoNsu, bool validarXmlConsulta = true)
        {
            logger.LogInformation($"Consultar documentos destinados, ultimo NSU {ultimoNsu}");
            if (string.IsNullOrEmpty(ultimoNsu))
                throw new ArgumentNullException(nameof(ultimoNsu));
            if (ultimoNsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(ultimoNsu));

            ultimoNsu = ultimoNsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                DistNSU = new distNSU
                {
                    UltNSU = ultimoNsu
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt, validarXmlConsulta);
             var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorNSUAsync(string nsu, bool validarXmlConsulta = true)
        {
            logger.LogInformation($"Consultar documentos destinados por NSU {nsu}");
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));
            if (nsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(nsu));

            nsu = nsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                consNSU = new consNSU { NSU = nsu },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt, validarXmlConsulta);
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorChaveAsync(ChaveFiscal chaveNFe, bool validarXmlConsulta = true)
        {
            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe.Chave
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt, validarXmlConsulta);
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);

            return retDistDFeInt;
        }
    }
}
