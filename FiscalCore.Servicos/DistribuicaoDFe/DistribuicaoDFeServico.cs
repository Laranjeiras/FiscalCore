using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.DistribuicaoDFe.Schemas;
using FiscalCore.Configuracoes;
using FiscalCore.Fabrica;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Servicos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using FiscalCore.Validacoes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiscalCore.DistribuicaoDFe.Servicos
{
    public class DistribuicaoDFeServico
    {
        private readonly IConfiguracaoServico cfgServico;

        public DistribuicaoDFeServico(IConfiguracaoServico cfgServico)
        {
            this.cfgServico = cfgServico;
        }

        public async Task<string> ConsultarEmissaoContraCNPJ(string nsu, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));
            if (nsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(nsu));

            nsu = nsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = cfgServico.Emitente.CNPJ,
                DistNSU = new DistNSU
                {
                    UltNSU = nsu
                },
                cUFAutor = ((int)cfgServico.UF).ToString(),
                TpAmb = cfgServico.TipoAmbiente
            };

            var xml = Xml.ClasseParaXmlString<distDFeInt>(distDFeInt);

            if (validarXmlConsulta)
                Schemas.ValidarSchema(eTipoServico.NFeDistribuicaoDFe, xml, cfgServico);

            Xml.SalvarArquivoXml(cfgServico.DiretorioSalvarXml, DateTime.Now.Ticks + "-ped-DistDFeInt.xml", xml);

            var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.NFeDistribuicaoDFe, xml);

            var sefazUrl = SefazServico.ObterUrl(eTipoServico.ManifestacaoDestinatario, eTipoAmbiente.Producao, eModeloDocumento.NFe, eUF.RJ);
            var retorno = await SefazServico.EnviarParaSefazAsync(cfgServico, sefazUrl, envelope);

            var retornoLimpo = Soap.LimparEnvelope(retorno, "retDistDFeInt").OuterXml;

            Xml.SalvarArquivoXml(cfgServico.DiretorioSalvarXml, $"{cfgServico.Emitente.CNPJ ?? cfgServico.Emitente.CPF}-{DateTime.Now.Ticks}-retDistDFeInt.xml", retornoLimpo);

            return retornoLimpo;
        }

        public async Task<retDistDFeInt> MontarRetorno(string retorno)
        {
            var retDistDFeInt = Xml.XmlStringParaClasse<retDistDFeInt>(retorno);

            var conteudos = new List<RetornoT>();

            if (retDistDFeInt.loteDistDFeInt != null)
            {
                foreach (var dfeInt in retDistDFeInt.loteDistDFeInt)
                {
                    var tmpSchema = dfeInt.schema.Split("_");
                    var tipoRet = tmpSchema[0] ?? dfeInt.schema;

                    var conteudoZip = Arquivo.Unzip(dfeInt.XmlNfe);
                    await Xml.SalvarArquivoXmlAsync(cfgServico.DiretorioSalvarXml, $"{cfgServico.Emitente.CNPJ ?? cfgServico.Emitente.CPF}-{DateTime.Now.Ticks}-{dfeInt.NSU}-{tipoRet}.xml", conteudoZip);

                    var conteudo = new RetornoT();

                    if (conteudoZip.StartsWith("<resNFe"))
                    {
                        var retConteudo = Xml.XmlStringParaClasse<DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.DistribuicaoDFe.Schemas.resNFe>(conteudoZip);

                        conteudo.Chave = retConteudo.chNFe;
                        conteudo.Mensagem = $"Conteúdo da NFE: {retConteudo.CNPJ ?? retConteudo.CPF} {retConteudo.xNome}";
                    }
                    else if (conteudoZip.StartsWith("<procEventoNFe"))
                    {
                        var xml = Xml.ObterTagXml(conteudoZip, "procEventoNFe");
                        var retEvento = Xml.XmlStringParaClasse<procEventoNFe>(xml);
                        conteudo.Chave = retEvento.retEvento.infEvento.chNFe;
                        conteudo.Mensagem = retEvento.retEvento.infEvento.xMotivo;
                    }
                    conteudos.Add(conteudo);
                }
            }
            return retDistDFeInt;
        }
    }

    public class RetornoT
    {
        public string Chave { get; set; }
        public string Mensagem { get; set; }
        public string Cliente { get; set; }
    }
}
