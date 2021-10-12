using FiscalCore.Configuracoes;
using FiscalCore.Servicos;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FiscalCore.Testes
{
    [TestClass]
    public class AutorizarNFeTeste : ContainerDI
    {
        private readonly IAutorizarNFeServico autServico;
        private readonly ConfiguracaoServico configServico;

        public AutorizarNFeTeste()
        {
            this.autServico = (IAutorizarNFeServico)ServiceProvider.GetService(typeof(IAutorizarNFeServico));
            this.configServico = (ConfiguracaoServico)ServiceProvider.GetService(typeof(ConfiguracaoServico));
        }

        [TestMethod]
        public async Task Autorizar()
        {

            var nfe = new NFes.NFe_1(configServico).ObterNFe();
            try
            {
                var retorno = await autServico.Autorizar(nfe);
                Assert.IsTrue(retorno.Autorizado, "NFe não autorizada");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ChaveAcesso()
        {
            var chave = new ChaveFiscal(eUF.RJ, new DateTime(2020, 12, 01), "29310114000110", eModeloDocumento.NFCe, 1, 273, eTipoEmissao.Normal, "00028034");
            Assert.AreEqual("33201229310114000110650010000002731000280340", chave.Chave);
        }
    }
}
