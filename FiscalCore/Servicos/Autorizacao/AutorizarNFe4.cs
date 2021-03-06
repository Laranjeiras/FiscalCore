using FiscalCore.Configuracoes;
using FiscalCore.Enums;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using FiscalCore.Servicos.Utils;
using System;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Extensions;
using DFeBR.EmissorNFe.Utilidade.Tipos;
using System.Collections.Generic;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.RetornoServicos.Autorizacao;
using System.Linq;
using FiscalCore.Validacoes;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4 : IAutorizarNFe
    {
        private readonly ConfiguracaoServico cfgServico;

        public AutorizarNFe4(ConfiguracaoServico cfgServico)
        {
            this.cfgServico = cfgServico;
        }

        public IRetornoAutorizacao Autorizar(NFe nfe, int idLote = 0)
        {
            var lista = new List<NFe> { nfe };
            return Autorizar(lista, idLote);
        }

        public IRetornoAutorizacao Autorizar(IList<NFe> nfes, int idLote = 0)
        {
            new ValidarNFeAutorizacao(nfes, cfgServico).Validar();
            new TratarNFeAutorizacao(ref nfes, cfgServico).Tratar();

            if(idLote <= 0)
                idLote = new Random().Next(10000000, 99999999);

            var nfesAssinadas = new List<NFe>();

            foreach (var nfe in nfes)
            {
                var nfeAssinada = nfe.Assinar(cfgServico.ConfigCertificado.ObterCertificado());
                Schemas.ValidarSchema(nfeAssinada, TipoServico.AutorizarNFe, cfgServico);
                nfesAssinadas.Add(nfeAssinada);
            }

            var mod = nfes.Select(x=>x.infNFe.ide.mod)
                .Distinct()
                .SingleOrDefault()
                .Parse();

            var versaoServico = cfgServico.VersaoAutorizacaoNFe.Descricao();
            var enviNFe = new enviNFe(versaoServico, idLote, IndicadorSincronizacao.Sincrono, nfesAssinadas);

            return Autorizar(enviNFe.String(), mod);
        }

        private IRetornoAutorizacao Autorizar(string xmlenviNFe4, eModeloDocumento modeloDocumento)
        {
            FuncoesXml.SalvarArquivoXml($"{cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-env-nfe.xml", xmlenviNFe4);

            var urlSefaz = ObterSefazUrl.ObterUrl(TipoServico.AutorizarNFe, cfgServico.TipoAmbiente, modeloDocumento, cfgServico.UF);

            var envelope = SoapEnvelopes.FabricarEnvelopeAutorizacaoNFe4(xmlenviNFe4);

            var retornoXmlString = Sefaz.EnviarParaSefaz(cfgServico, urlSefaz, envelope);
            var retornoLimpo = Soap.ClearEnvelop(retornoXmlString, "retEnviNFe").OuterXml;

            FuncoesXml.SalvarArquivoXml($"{cfgServico.DiretorioSalvarXml}", $"{DateTime.Now.Ticks}-ret-env-nfe.xml", retornoLimpo);

            var retEnviNFe = new RetNFeAutorizacao4(retornoLimpo);
            retEnviNFe.XmlEnviado = xmlenviNFe4;
            return retEnviNFe;
        }
    }
}
