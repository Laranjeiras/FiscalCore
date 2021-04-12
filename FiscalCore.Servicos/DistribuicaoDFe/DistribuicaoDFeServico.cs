using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
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

        public async Task<string> ConsultarDocumentosDestinadosAsync(string ultimoNsu, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(ultimoNsu))
                throw new ArgumentNullException(nameof(ultimoNsu));
            if (ultimoNsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(ultimoNsu));

            ultimoNsu = ultimoNsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.Emitente.CNPJ,
                DistNSU = new distNSU
                {
                    UltNSU = ultimoNsu
                },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            return await TransmitirSefaz(distDFeInt, validarXmlConsulta);
        }

        public async Task<string> ConsultarDFePorNSU(string nsu)
        {
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = configuracao.Emitente.CNPJ,
                consNSU = new consNSU { NSU = nsu },
                cUFAutor = ((int)configuracao.UF).ToString(),
                TpAmb = configuracao.TipoAmbiente
            };

            return await TransmitirSefaz(distDFeInt);
        }

        public async Task<string> ConsultarDFePorChaveAsync(string chaveNFe, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(chaveNFe))
                throw new ArgumentNullException(nameof(chaveNFe));
            if (!(chaveNFe.Length == 44))
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

            return await TransmitirSefaz(distDFeInt, validarXmlConsulta);
        }

        private async Task<string> TransmitirSefaz(distDFeInt distDFeInt, bool validarXmlConsulta = true)
        {
            var xml = XmlUtils.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (validarXmlConsulta)
                Schemas.ValidarSchema(eTipoServico.NFeDistribuicaoDFe, xml, configuracao);

            await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-ped-DistDFeInt.xml", xml);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.NFeDistribuicaoDFe, configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
            var retorno = await SefazServico.EnviarParaSefazAsync(configuracao, sefazUrl, envelope);

            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;

            await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-retDistDFeInt.xml", retornoLimpo);

            return retornoLimpo;
        }

        public async Task<RetDistDFeDTO> MontarRetorno(string retorno)
        {
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);

            var retornoDto = new RetDistDFeDTO
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

                    string chaveNFe = null;
                    string tipoRetorno = null;

                    await Arquivo.SalvarArquivoAsync(configuracao, "DistribuicaoDFe", $"{configuracao.Emitente.CNPJ ?? configuracao.Emitente.CPF}-{DateTime.Now.Ticks}-{dfeInt.NSU}-{tipoRet}.xml", conteudoZip);

                    if (conteudoZip.StartsWith("<resNFe"))
                    {
                        var retConteudo = XmlUtils.XmlStringParaClasse<resNFe>(conteudoZip);
                        retornoDto.ResNFes.Add(retConteudo);
                        chaveNFe = retConteudo.chNFe;
                        tipoRetorno = "resNFe";
                    }
                    else if (conteudoZip.StartsWith("<procEventoNFe"))
                    {
                        var xml = XmlUtils.ObterTagXml(conteudoZip, "procEventoNFe");
                        var procEvento = XmlUtils.XmlStringParaClasse<procEventoNFe>(xml);
                        retornoDto.ProcEventos.Add(procEvento);
                        chaveNFe = procEvento?.retEvento?.infEvento?.chNFe;
                        tipoRetorno = "procEventoNFe";
                    }
                    else if (conteudoZip.StartsWith("<nfeProc"))
                    {
                        var xml = XmlUtils.ObterTagXml(conteudoZip, "nfeProc");
                        var nfeProc = XmlUtils.XmlStringParaClasse<nfeProc>(xml);
                        retornoDto.NFeProcs.Add(nfeProc);
                        chaveNFe = nfeProc.NFe.infNFe.Id;
                        tipoRetorno = "nfeProc";
                    }

                    retornoDto.DistDfeIntDTOs.Add(new RetDistDFeIntDTO(chaveNFe, dfeInt.NSU, conteudoZip, tipoRetorno));
                }
            }
            return retornoDto;
        }
    }
}
