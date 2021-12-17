using FiscalCore.Configuracoes;
using FiscalCore.DistribuicaoDFe.Servicos;
using FiscalCore.Exceptions;
using FiscalCore.Servicos;
using FiscalCore.Servicos.NotaFiscal;
using FiscalCore.Testes.NFes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.Json;

namespace FiscalCore.Testes
{
    public class ContainerDI 
    { 
        protected ServiceProvider ServiceProvider { get; private set; }

        public ContainerDI()
        {
            var config = JsonSerializer.Deserialize<ConfiguracaoServico>(File.ReadAllText("appsettings.json"));            
            try
            {
                config.Validar();
            }
            catch (FalhaValidacaoException ex)
            {
                Assert.Fail($"\nArquivo appsettings.json inválido\n{ex.Message}");
            }

            var services = new ServiceCollection();
            services.AddSingleton<ConfiguracaoServico>(config);
            services.AddScoped<DistribuicaoDFeServico>();
            services.AddScoped<SefazServico>();
            services.AddScoped<INFeExemplosTeste, NFe_1>();
            services.AddScoped<IAutorizarNFeServico, AutorizarNFe4>();
            services.AddScoped<NotaFiscalServico>();
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
