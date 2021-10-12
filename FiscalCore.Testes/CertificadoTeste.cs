using FiscalCore.Configuracoes;
using FiscalCore.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FiscalCore.Testes
{
    [TestClass]
    public class CertificadoTeste : ContainerDI
    {
        private readonly ConfiguracaoServico configServico;

        public CertificadoTeste()
        {
            this.configServico = (ConfiguracaoServico)ServiceProvider.GetService(typeof(ConfiguracaoServico));
        }

        [TestMethod]
        public void VerificarCnpj()
        {
            var cert = ObterCertificado.Obter(configServico.ConfigCertificado);
            var cpfCnpj = ObterCertificado.ExtrairCNPJArquivo(cert);
            System.Console.WriteLine(cert.Validade());
            Assert.IsTrue(DateTime.Now < cert.NotAfter);
        }
    }
}
