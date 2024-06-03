using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Inutilizacao;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using FiscalCore.Fabrica;
using System.IO;
using AlgoPlus.Storage.Services;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class InutilizarNFeServico : BaseSefazServico<InutilizarNFeServico>, IEventoServico
    {
        public InutilizarNFeServico(ConfiguracaoServico cfgServico, IStorageContext storageContext, ITransmitirSefazCommand transmitir, ILogger<InutilizarNFeServico> logger) 
            : base(cfgServico, transmitir, logger, storageContext) 
        {
        }

        public async Task<retInutNFe> Inutilizar(int ano, eModeloDocumento modeloDocumento, int serie, int numeroInicial, int numeroFinal, string justificativa, CancellationToken cancellation) 
        {
            var cnpj = configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF;
            var tpAmb = configuracao.TipoAmbiente;
            var uf = configuracao.UF;
            var pedInutilizacao = FabricarInutNFe(tpAmb, uf, ano, cnpj, modeloDocumento, serie, numeroInicial, numeroFinal, justificativa);
                        
            var xmlInutilizacao = XmlUtils.ClasseParaXmlString<inutNFe>(pedInutilizacao);

            var arqEnv = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ped-inut.xml", configuracao));
            await SalvarLog(arqEnv, xmlInutilizacao, cancellation);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.InutilizacaoNFe, xmlInutilizacao);
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.InutilizacaoNFe, configuracao.TipoAmbiente, modeloDocumento, configuracao.UF);
            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retInutNFe").OuterXml;

            var arqRet = Path.Combine("Logs", Arquivo.MontarNomeArquivo("ret-inut.xml", configuracao));
            await SalvarLog(arqRet, retornoLimpo, cancellation);

            return XmlUtils.XmlStringParaClasse<retInutNFe>(retornoLimpo);
        }

        private inutNFe FabricarInutNFe(eTipoAmbiente tpAmb, eUF uf, int ano, string cnpj, eModeloDocumento modelo, int serie, int numeroInicial, int numeroFinal, string justificativa)
        {
            string versaoServico = configuracao.VersaoInutilizacaoNFe.Descricao();

            var pedInutilizacao = new inutNFe
            {
                versao = versaoServico,
                infInut = new infInutEnv
                {
                    tpAmb = tpAmb,
                    cUF = uf,
                    ano = ano,
                    CNPJ = cnpj,
                    mod = modelo,
                    serie = serie,
                    nNFIni = numeroInicial,
                    nNFFin = numeroFinal,
                    xJust = justificativa
                }
            };

            var numId = string.Concat(
                (int)pedInutilizacao.infInut.cUF,
                pedInutilizacao.infInut.ano.ToString("D2"),
                pedInutilizacao.infInut.CNPJ, (int)pedInutilizacao.infInut.mod,
                pedInutilizacao.infInut.serie.ToString().PadLeft(3, '0'),
                pedInutilizacao.infInut.nNFIni.ToString().PadLeft(9, '0'),
                pedInutilizacao.infInut.nNFFin.ToString().PadLeft(9, '0')
            );
            pedInutilizacao.infInut.Id = "ID" + numId;

            pedInutilizacao.Assinar(configuracao.ConfigCertificado.Certificado);

            return pedInutilizacao;
        }
    }
}
