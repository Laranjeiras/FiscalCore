using FiscalCore.Configuracoes;
using FiscalCore.Servicos.NotaFiscal;
using FiscalCore.Servicos.NotaFiscal.Eventos;
using FiscalCore.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiscalCore.Testes
{
    [TestClass]
    public class CancelarNFeTeste : ContainerDI
    {
        private readonly NotaFiscalServico nfServico;
        private readonly ConfiguracaoServico configServico;

        public CancelarNFeTeste()
        {
            this.nfServico = (NotaFiscalServico)ServiceProvider.GetService(typeof(NotaFiscalServico));
            this.configServico = (ConfiguracaoServico)ServiceProvider.GetService(typeof(ConfiguracaoServico));
        }

        [TestMethod]
        public async Task CancelarNFe()
        {
            var chave = new ChaveFiscal("33211029310114000110550010000000041000000546");
            var protocolo = new ProtocoloAutorizacao("333210000440983");
            var infoNFe = new InfoNFeCancelar(chave, protocolo);
            var retorno = await nfServico.CancelarNFe.Cancelar(infoNFe);
            Console.WriteLine(retorno.XmlRecebido);
        }
    }
}
