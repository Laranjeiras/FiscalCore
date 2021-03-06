using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Extensions;
using FiscalCore.Modelos.Inutilizacao;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;
using Zion.Common.Extensions;

namespace FiscalCore.Servicos
{
    public class InutilizarNFeServico
    {
        ConfiguracaoServico _cfgServico;

        public InutilizarNFeServico(ConfiguracaoServico cfgServico)
        {
            _cfgServico = cfgServico;
        }

        public retInutNFe Inutilizar(int ano, eModeloDocumento modeloDocumento, int serie, int numeroInicial, int numeroFinal, string justificativa) 
        {
            var cnpj = _cfgServico.Emitente.CNPJ ?? _cfgServico.Emitente.CPF;
            var tpAmb = _cfgServico.TipoAmbiente;
            var uf = _cfgServico.UF;
            var pedInutilizacao = FabricarInutNFe(tpAmb, uf, ano, cnpj, modeloDocumento, serie, numeroInicial, numeroFinal, justificativa);
                        
            var xmlInutilizacao = pedInutilizacao.ObterXmlString();

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, $"{DateTime.Now.Ticks} - {pedInutilizacao.infInut.Id} -ped-inut.xml", xmlInutilizacao);

            var envelope = SoapEnvelopes.FabricarEnvelopeInutilizacaoNFe(xmlInutilizacao);
            var sefazUrl = ObterSefazUrl.ObterUrl(TipoServico.InutilizacaoNFe, _cfgServico.TipoAmbiente, modeloDocumento, _cfgServico.UF);
            var retornoXmlString = Sefaz.EnviarParaSefaz(_cfgServico, sefazUrl, envelope);
            var retornoLimpo = Soap.ClearEnvelop(retornoXmlString, "retInutNFe").OuterXml;

            FuncoesXml.SalvarArquivoXml(_cfgServico.DiretorioSalvarXml, $"{DateTime.Now.Ticks} - {pedInutilizacao.infInut.Id} -inut.xml", retornoLimpo);

            return new retInutNFe().CarregarDeXmlString(retornoLimpo);
        }

        private inutNFe FabricarInutNFe(eTipoAmbiente tpAmb, eUF uf, int ano, string cnpj, eModeloDocumento modelo, int serie, int numeroInicial, int numeroFinal, string justificativa)
        {
            Zion.Common.Assertions.ZionAssertion.StringHasMinLen(justificativa, 15, "Justificativa deve conter entre 15 e 255 caracteres");
            Zion.Common.Assertions.ZionAssertion.StringHasMaxLen(justificativa, 255, "Justificativa deve conter entre 15 e 255 caracteres");

            string versaoServico = _cfgServico.VersaoInutilizacaoNFe.GetDescription();

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

            pedInutilizacao.Assina(ObterCertificado.ObterCertificado(_cfgServico.ConfigCertificado.Serial));

            return pedInutilizacao;
        }
    }
}
