using FiscalCore.Configuracoes;
using FiscalCore.DTOs.DistribuicaoDFe;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.Consulta;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Servicos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FiscalCore.DistribuicaoDFe.Servicos
{
    public class DistribuicaoDFeServico
    {
        private readonly IConfiguracaoServico configuracao;

        public DistribuicaoDFeServico(IConfiguracaoServico configuracao)
        {
            this.configuracao = configuracao;
        }

        public async Task<string> ConsultarDistribuicao(string nsu, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));
            if (nsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(nsu));

            nsu = nsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.Emitente.CNPJ,
                DistNSU = new distNSU
                {
                    UltNSU = nsu
                },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (validarXmlConsulta)
                Schemas.ValidarSchema(eTipoServico.NFeDistribuicaoDFe, xml, configuracao);

            await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml", xml);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.NFeDistribuicaoDFe, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.RJ);
            var retorno = await SefazServico.EnviarParaSefazAsync(configuracao, sefazUrl, envelope);

            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;

            await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-retDistDFeInt.xml", xml);

            return retornoLimpo;
        }

        public async Task<string> ConsultarDFe(string chaveNFe, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(chaveNFe))
                throw new ArgumentNullException(nameof(chaveNFe));
            if (!(chaveNFe.Length == 43))
                throw new ArgumentOutOfRangeException(nameof(chaveNFe));

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.Emitente.CNPJ,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe
                },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            return null;
        }

        public async Task<RetDistNFeDTO> MontarRetorno(string retorno)
        {
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);

            var retornoDto = new RetDistNFeDTO
            {
                CStat = retDistDFeInt.cStat,
                Motivo = retDistDFeInt.xMotivo
            };

            if (retDistDFeInt.loteDistDFeInt != null)
            {
                foreach (var dfeInt in retDistDFeInt.loteDistDFeInt)
                {
                    var tmpSchema = dfeInt.schema.Split("_");
                    var tipoRet = tmpSchema[0] ?? dfeInt.schema;

                    var conteudoZip = Arquivo.Unzip(dfeInt.XmlNfe);

                    await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-{dfeInt.NSU}-{tipoRet}.xml", conteudoZip);

                    if (conteudoZip.StartsWith("<resNFe"))
                    {
                        var retConteudo = XmlUtils.XmlStringParaClasse<resNFe>(conteudoZip);
                        retornoDto.ResNFes.Add(retConteudo);
                        
                    }
                    else if (conteudoZip.StartsWith("<procEventoNFe"))
                    {
                        var xml = XmlUtils.ObterTagXml(conteudoZip, "procEventoNFe");
                        var retEvento = XmlUtils.XmlStringParaClasse<procEventoNFe>(xml);
                        retornoDto.ProcEventos.Add(retEvento);
                    }
                }
            }
            return retornoDto;
        }
    }
}
