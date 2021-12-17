using FiscalCore.Configuracoes;
using FiscalCore.Modelos.DistribuicaoDFe;
using FiscalCore.Servicos;
using FiscalCore.Utils;
using FiscalCore.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FiscalCore.DistribuicaoDFe.Servicos
{
    public class DistribuicaoDFeServico : BaseSefazServico
    {
        private readonly ILogger logger;

        public DistribuicaoDFeServico(ConfiguracaoServico configuracao, ITransmitirSefazCommand transmitir, ILogger<DistribuicaoDFeServico> logger)
            : base(configuracao, transmitir)
        {
            this.logger = logger;
        }

        public async Task<retDistDFeInt> ConsultarDocumentosDestinadosAsync(string ultimoNsu, bool validarXmlConsulta = true)
        {
            logger.LogInformation($"Consultar documentos destinados, ultimo NSU {ultimoNsu}");
            if (string.IsNullOrEmpty(ultimoNsu))
                throw new ArgumentNullException(nameof(ultimoNsu));
            if (ultimoNsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(ultimoNsu));

            ultimoNsu = ultimoNsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                DistNSU = new distNSU
                {
                    UltNSU = ultimoNsu
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt, validarXmlConsulta);
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarPorNSUAsync(string nsu)
        {
            logger.LogInformation($"Consultar documentos destinados por NSU {nsu}");
            if (string.IsNullOrEmpty(nsu))
                throw new ArgumentNullException(nameof(nsu));
            if (nsu.Length > 15)
                throw new ArgumentOutOfRangeException(nameof(nsu));

            nsu = nsu.PadLeft(15, '0');

            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                consNSU = new consNSU { NSU = nsu },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt);
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);
            return retDistDFeInt;
        }

        public async Task<retDistDFeInt> ConsultarDFePorChaveAsync(ChaveFiscal chaveNFe, bool validarXmlConsulta = true)
        {
            var distDFeInt = new distDFeInt
            {
                Versao = "1.01",
                Cnpj = Configuracao.Emitente.CNPJ,
                consChNFe = new consChNFe
                {
                    ChNFe = chaveNFe.Chave
                },
                cUFAutor = ((int)Configuracao.UF).ToString(),
                TpAmb = Configuracao.TipoAmbiente
            };

            var retorno = await Transmitir.TransmitirAsync(distDFeInt, validarXmlConsulta);
            var retDistDFeInt = XmlUtils.XmlStringParaClasse<retDistDFeInt>(retorno);

            return retDistDFeInt;
        }

        //private async Task<string> ManifestarAsync(string chaveAcesso, eTipoEventoNFe tipoEvento, string justificativa = null)
        //{
        //    var xmlEvento = GerarXmlEvento(chaveAcesso, tipoEvento, justificativa);

        //    await Arquivo.SalvarArquivoAsync(Configuracao, "Eventos", $"{DateTime.Now.Ticks}-ped-eve.xml", xmlEvento);

        //    var envelope = SoapEnvelopeFabrica.FabricarEnvelope(eTipoServico.ManifestacaoDestinatario, xmlEvento);
        //    var sefazUrl = FabricarUrl.ObterUrl(eTipoServico.ManifestacaoDestinatario, Configuracao.TipoAmbiente, eModeloDocumento.NFe, eUF.AN);
        //    var xmlRetorno = await Transmitir.ExecutarAsync(sefazUrl, envelope);
        //    var xmlRetLimpo = Soap.LimparEnvelope(xmlRetorno, "retEnvEvento").OuterXml;
        //    await Arquivo.SalvarArquivoAsync(Configuracao, "Eventos", $"{DateTime.Now.Ticks}-ret-eve.xml", xmlRetLimpo);
        //    return xmlRetLimpo;
        //}

        //private RetEventoManifestacaoDTO MontarDTO(string xml)
        //{
        //    var retEnv = XmlUtils.XmlStringParaClasse<retEnvEvento>(xml);
        //    if (retEnv.retEvento.Count != 1)
        //        throw new Exception("Retorno com mais de 1 evento registrado");

        //    var inf = retEnv.retEvento.SingleOrDefault();
        //    return new RetEventoManifestacaoDTO(inf.infEvento);
        //}

        //private string GerarXmlEvento(string chaveAcesso, eTipoEventoNFe tipoEvento, string justificativa)
        //{
        //    if (tipoEvento != eTipoEventoNFe.OperacaoNaoRealizada)
        //        justificativa = null;
        //    else
        //        throw new ArgumentNullException(nameof(justificativa));

        //    var infEvento = new infEventoEnv
        //    {
        //        chNFe = chaveAcesso,
        //        CNPJ = Configuracao.Emitente.CNPJ,
        //        CPF = Configuracao.Emitente.CPF,
        //        cOrgao = eUF.AN,
        //        dhEvento = DateTime.Now,
        //        nSeqEvento = nSeqEvento,
        //        tpAmb = Configuracao.TipoAmbiente,
        //        tpEvento = tipoEvento,
        //        verEvento = versao,
        //        Id = "ID" + ((int)tipoEvento) + chaveAcesso + nSeqEvento.ToString().PadLeft(2, '0'),
        //        detEvento = new detEvento()
        //        {
        //            versao = versao,
        //            descEvento = tipoEvento.Descricao().RemoverAcentos()
        //        }
        //    };

        //    var evento = new evento
        //    {
        //        versao = versao,
        //        infEvento = infEvento
        //    };

        //    evento.Assinar(ObterCertificado.Obter(Configuracao.ConfigCertificado), Configuracao.ConfigCertificado.SignatureMethodSignedXml, Configuracao.ConfigCertificado.DigestMethodReference);

        //    var pedEvento = new envEvento
        //    {
        //        versao = versao,
        //        idLote = 1,
        //        evento = new List<evento> { evento }
        //    };

        //    var xmlEvento = XmlUtils.ClasseParaXmlString<envEvento>(pedEvento);
        //    return xmlEvento;
        //}
    }
}
