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
using resNFe = FiscalCore.Modelos.DistribuicaoDFe.resNFe;

namespace FiscalCore.DistribuicaoDFe.Servicos
{
    public class DistribuicaoDFeServico
    {
        private readonly IConfiguracaoServico cfgServico;

        public DistribuicaoDFeServico(IConfiguracaoServico cfgServico)
        {
            this.cfgServico = cfgServico;
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

        public async Task<string> ConsultarDFe(string chaveNFe, bool validarXmlConsulta = true)
        {
            if (string.IsNullOrEmpty(chaveNFe))
                throw new ArgumentNullException(nameof(chaveNFe));
            if (!(chaveNFe.Length == 43))
                throw new ArgumentOutOfRangeException(nameof(chaveNFe));

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = cfgServico.Emitente.CNPJ,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe
                },
                cUFAutor = ((int)cfgServico.UF).ToString(),
                TpAmb = cfgServico.TipoAmbiente
            };

            return null;
        }

        public async Task<RetornoDistNFeViewModel> MontarRetorno(string retorno)
        {
            var retDistDFeInt = Xml.XmlStringParaClasse<retDistDFeInt>(retorno);

            var retornoViewModel = new RetornoDistNFeViewModel();
            retornoViewModel.CStat = retDistDFeInt.cStat;
            retornoViewModel.XMotivo = retDistDFeInt.xMotivo;

            if (retDistDFeInt.loteDistDFeInt != null)
            {
                foreach (var dfeInt in retDistDFeInt.loteDistDFeInt)
                {
                    var tmpSchema = dfeInt.schema.Split("_");
                    var tipoRet = tmpSchema[0] ?? dfeInt.schema;

                    var conteudoZip = Arquivo.Unzip(dfeInt.XmlNfe);
                    await Xml.SalvarArquivoXmlAsync(cfgServico.DiretorioSalvarXml, $"{cfgServico.Emitente.CNPJ ?? cfgServico.Emitente.CPF}-{DateTime.Now.Ticks}-{dfeInt.NSU}-{tipoRet}.xml", conteudoZip);

                    if (conteudoZip.StartsWith("<resNFe"))
                    {
                        var retConteudo = Xml.XmlStringParaClasse<resNFe>(conteudoZip);
                        retornoViewModel.ResNFes.Add(retConteudo);
                        
                    }
                    else if (conteudoZip.StartsWith("<procEventoNFe"))
                    {
                        var xml = Xml.ObterTagXml(conteudoZip, "procEventoNFe");
                        var retEvento = Xml.XmlStringParaClasse<procEventoNFe>(xml);
                        retornoViewModel.ProcEventos.Add(retEvento);
                    }
                }
            }
            return retornoViewModel;
        }
    }

    public class RetornoDistNFeViewModel
    {
        public RetornoDistNFeViewModel()
        {
            ResNFes = new List<resNFe>();
            ProcEventos = new List<procEventoNFe>();
        }

        public IList<resNFe> ResNFes { get; private set; }
        public IList<procEventoNFe> ProcEventos { get; private set; }

        public int CStat { get; set; }
        public string XMotivo { get; set; }
    }
}
