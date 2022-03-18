using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.NotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Servicos
{
    internal class ValidarNFeAutorizacao
    {
        private readonly IList<NFe> nfes;
        private readonly ConfiguracaoServico cfgServico;

        public ValidarNFeAutorizacao(IList<NFe> nfes, ConfiguracaoServico cfgServico)
        {
            this.nfes = nfes;
            this.cfgServico = cfgServico;
        }

        public void Validar()
        {
            if (!ValidarEmitenteEmissor(nfes))
                throw new FalhaValidacaoException("CNPJ/CPF do Emitente da NFe difere do CNPJ/CPF do Emissor Cadastrado");

            if (!ValidarModeloUnico(nfes))
                throw new FalhaValidacaoException("Apenas um modelo de NFe pode ser enviado por lote");
        }

        private bool ValidarEmitenteEmissor(IList<NFe> nfes)
        {
            try
            {
                var cpfCnpjEmitente = nfes.Select(x => new { CpfCnpj = x.infNFe.emit.CNPJ ?? x.infNFe.emit.CPF }).Distinct().SingleOrDefault();
                return !(cpfCnpjEmitente.CpfCnpj != cfgServico.Emitente.CpfCnpj);
            }
            catch
            {
                return false;
            }
        }

        private bool ValidarModeloUnico(IList<NFe> nfes)
        {
            var contar = nfes.Select(x => x.infNFe.ide.mod).Distinct().Count();
            return contar == 1;
        }
    }
}
