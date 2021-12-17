using FiscalCore.Configuracoes;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Inutilizacao;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using FiscalCore.Fabrica;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class InutilizarNFeServico : IEventoServico
    {
        private ConfiguracaoServico cfgServico;
        private readonly ITransmitirSefazCommand transmitir;

        public InutilizarNFeServico(ConfiguracaoServico cfgServico, ITransmitirSefazCommand transmitir)
        {
            this.cfgServico = cfgServico;
            this.transmitir = transmitir;
        }

        public async Task<retInutNFe> Inutilizar(int ano, eModeloDocumento modeloDocumento, int serie, int numeroInicial, int numeroFinal, string justificativa) 
        {
            var cnpj = cfgServico.Emitente.CNPJ ?? cfgServico.Emitente.CPF;
            var tpAmb = cfgServico.TipoAmbiente;
            var uf = cfgServico.UF;
            var pedInutilizacao = FabricarInutNFe(tpAmb, uf, ano, cnpj, modeloDocumento, serie, numeroInicial, numeroFinal, justificativa);
                        
            var xmlInutilizacao = XmlUtils.ClasseParaXmlString<inutNFe>(pedInutilizacao);

            await Arquivo.SalvarArquivoAsync(cfgServico.DiretorioSalvarXml, $"{DateTime.Now.Ticks} - {pedInutilizacao.infInut.Id} -ped-inut.xml", xmlInutilizacao);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.InutilizacaoNFe, xmlInutilizacao);
            var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.InutilizacaoNFe, cfgServico.TipoAmbiente, modeloDocumento, cfgServico.UF);
            var retornoXmlString = await transmitir.TransmitirAsync(sefazUrl, envelope);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retInutNFe").OuterXml;

            await Arquivo.SalvarArquivoAsync(cfgServico.DiretorioSalvarXml, $"{DateTime.Now.Ticks} - {pedInutilizacao.infInut.Id} -inut.xml", retornoLimpo);

            return XmlUtils.XmlStringParaClasse<retInutNFe>(retornoLimpo);
        }

        private inutNFe FabricarInutNFe(eTipoAmbiente tpAmb, eUF uf, int ano, string cnpj, eModeloDocumento modelo, int serie, int numeroInicial, int numeroFinal, string justificativa)
        {
            //Zion.Common.Assertions.ZionAssertion.StringHasMinLen(justificativa, 15, "Justificativa deve conter entre 15 e 255 caracteres");
            //Zion.Common.Assertions.ZionAssertion.StringHasMaxLen(justificativa, 255, "Justificativa deve conter entre 15 e 255 caracteres");

            string versaoServico = cfgServico.VersaoInutilizacaoNFe.Descricao();

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

            pedInutilizacao.Assinar(ObterCertificado.Obter(cfgServico.ConfigCertificado));

            return pedInutilizacao;
        }
    }
}
