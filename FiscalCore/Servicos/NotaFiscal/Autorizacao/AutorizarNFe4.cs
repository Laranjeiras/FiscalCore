using FiscalCore.Configuracoes;
using FiscalCore.Modelos.Retornos;
using FiscalCore.Utils;
using System;
using FiscalCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiscalCore.Tipos;
using AlgoPlus.Storage.Services;
using FiscalCore.NotaFiscal.RetornoServicos.Autorizacao;
using FiscalCore.NotaFiscal;
using Microsoft.Extensions.Logging;
using FiscalCore.Servicos.NotaFiscal;
using System.Threading;
using FiscalCore.Servicos.NotaFiscal.Autorizacao;

namespace FiscalCore.Servicos
{
    public class AutorizarNFe4 : AutorizarNFe, IAutorizarNFeServico
    {
        private readonly string versaoServico = null!;

        public AutorizarNFe4(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir, IStorageContext storageContext, ILogger<AutorizarNFe4> logger)
            : base(configuracao, transmitir, logger, storageContext)
        {
            this.cancellation = new CancellationToken(); // PARA FUNCIONAR O STORAGE
            this.versaoServico = configuracao.VersaoAutorizacaoNFe.Descricao() ?? string.Empty;
        }

        public async Task<IRetornoAutorizacao> Autorizar(NFe nfe, CancellationToken cancellation, int idLote = 0)
        {
            logger?.LogDebug("RECEBENDO NFe {Nfe}", nfe);
            var lista = new List<NFe> { nfe };
            return  await Autorizar(lista, cancellation, idLote);
        }

        public async Task<IRetornoAutorizacao> Autorizar(IList<NFe> nfes, CancellationToken cancellation, int idLote = 0)
        {
            logger?.LogDebug($"RECEBENDO LISTA DE NFes TOTAL: {nfes.Count}");

            var modelo = ModeloDocumentoDaLista(nfes);

            configuracao.ConfigCertificado.Certificado.Validar();

            logger?.LogDebug($"TRATAR NFes");
            TratarNFeAutorizacao.AplicarPoliticas(nfes);
            logger?.LogDebug($"NFes TRATADAS");
            
            logger?.LogDebug($"VALIDAR NFes");
            ValidarNFeAutorizacao.ValidarPoliticas(nfes, configuracao);
            logger?.LogDebug($"NFes VALIDADAS");

            if (idLote <= 0)
                idLote = new Random().Next(10000000, 99999999);

            logger?.LogDebug($"NUMERO LOTE: {idLote}");

            var nfesAssinadas = nfes
                .Select(ValidarEAssinar)
                .ToList();
                
            var enviNFe = new enviNFe(
                versaoServico, 
                idLote, 
                eIndicadorSincronizacao.Sincrono, 
                nfesAssinadas);

            var xmlEnviNFe = XmlUtils.ClasseParaXmlString<enviNFe>(enviNFe);
            var retorno = await Autorizar(xmlEnviNFe, modelo, cancellation);
            return retorno;
        }

        public eModeloDocumento ModeloDocumentoDaLista(IList<NFe> nfes) 
            => nfes.Select(x => x.infNFe.ide.mod)
                .Distinct()
                .SingleOrDefault();

        public NFe ValidarEAssinar(NFe nfe)
        {
            logger?.LogDebug("ASSINAR NFe");

            var nfeAssinada = nfe.Assinar(configuracao.ConfigCertificado.Certificado);
            var xml = XmlUtils.ClasseParaXmlString<NFe>(nfeAssinada);
            xml = xml.Replace("xmlns=\"http://www.portalfiscal.inf.br/nfe\"", string.Empty);

            logger?.LogDebug($"NFe [{nfe.infNFe.Id}] ASSINADA");

            if (configuracao.ValidarXmlSchema)
            {
                ValidarXml(eTipoServico.AutorizarNFe, configuracao, xml);
                logger?.LogDebug($"NFe [{nfeAssinada.infNFe.Id}] VALIDADA");
            }
            
            return nfeAssinada;
        }
    }
}
