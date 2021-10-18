using FiscalCore.Configuracoes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiscalCore.Servicos.NotaFiscal.Eventos
{
    public class ComprovanteEntregaNFeServico : IEventoServico
    {
        private readonly ConfiguracaoServico configServico;

        public ComprovanteEntregaNFeServico(ConfiguracaoServico configServico)
        {
            this.configServico = configServico;
        }



    }
}
