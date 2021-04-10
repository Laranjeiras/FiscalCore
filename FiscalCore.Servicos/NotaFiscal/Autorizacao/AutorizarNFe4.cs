using FiscalCore.Configuracoes;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Extensions;
using DFeBR.EmissorNFe.Utilidade.Tipos;
using System.Collections.Generic;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.Autorizacao;
using System.Linq;
using FiscalCore.Validacoes;
using System.Threading.Tasks;
using FiscalCore.Tipos;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4 : IAutorizarNFe
    {
        private readonly ConfiguracaoServico cfgServico;

        public AutorizarNFe4(ConfiguracaoServico cfgServico)
        {
            this.cfgServico = cfgServico;
        }

        public async Task<IRetornoAutorizacao> Autorizar(NFe nfe, int idLote = 0)
        {
            var lista = new List<NFe> { nfe };
            return  await Autorizar(lista, idLote);
        }

        public async Task<IRetornoAutorizacao> Autorizar(IList<NFe> nfes, int idLote = 0)
        {
            new ValidarNFeAutorizacao(nfes, cfgServico).Validar();
            new TratarNFeAutorizacao(ref nfes, cfgServico).Tratar();

            if(idLote <= 0)
                idLote = new Random().Next(10000000, 99999999);

            var nfesAssinadas = new List<NFe>();

            foreach (var nfe in nfes)
            {
                var nfeAssinada = nfe.Assinar(ObterCertificado.Obter(cfgServico.ConfigCertificado));
                var xml = XmlUtils.ClasseParaXmlString<NFe>(nfeAssinada);
                Schemas.ValidarSchema(eTipoServico.AutorizarNFe, xml, cfgServico);
                nfesAssinadas.Add(nfeAssinada);
            }

            var mod = nfes.Select(x=>x.infNFe.ide.mod)
                .Distinct()
                .SingleOrDefault()
                .Parse();

            var versaoServico = cfgServico.VersaoAutorizacaoNFe.Descricao();
            var enviNFe = new enviNFe(versaoServico, idLote, IndicadorSincronizacao.Sincrono, nfesAssinadas);
            var xmlEnviNFe = XmlUtils.ClasseParaXmlString<enviNFe>(enviNFe);
            return await Autorizar(xmlEnviNFe, mod);
        }

        private async Task<IRetornoAutorizacao> Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento)
        {
            XmlUtils.SalvarArquivoXml($"{cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-env-nfe.xml", xmlenviNFe4);

            var urlSefaz = SefazServico.ObterUrl(eTipoServico.AutorizarNFe, cfgServico.TipoAmbiente, modeloDocumento, cfgServico.UF);

            var envelope = Fabrica.SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.AutorizarNFe, xmlenviNFe4);

            var retornoXmlString = await SefazServico.EnviarParaSefazAsync(cfgServico, urlSefaz, envelope);
            var retornoLimpo = Soap.LimparEnvelope(retornoXmlString, "retEnviNFe").OuterXml;

            XmlUtils.SalvarArquivoXml($"{cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-ret-env-nfe.xml", retornoLimpo);

            var retEnviNFe = new RetNFeAutorizacao4(retornoLimpo);
            retEnviNFe.XmlEnviado = xmlenviNFe4;
            return retEnviNFe;
        }
    }
}
