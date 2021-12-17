using FiscalCore.Configuracoes;
using FiscalCore.DistribuicaoDFe.Servicos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiscalCore.Testes
{
    [TestClass]
    public class NFeDistDFeInteresseTeste : ContainerDI
    {
        private readonly DistribuicaoDFeServico dfeServico;
        private readonly ConfiguracaoServico configServico;

        public NFeDistDFeInteresseTeste()
        {
            this.dfeServico = (DistribuicaoDFeServico)ServiceProvider.GetService(typeof(DistribuicaoDFeServico));
            this.configServico = (ConfiguracaoServico)ServiceProvider.GetService(typeof(ConfiguracaoServico));
        }

        [TestMethod]
        public async Task ConsultarDocumentosDestinadosAsync()
        {
            await dfeServico.ConsultarDocumentosDestinadosAsync("0");
        }
    }
}
