using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
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

        private bool ValidarEmitenteEmissor(IList<NFe> nfes)
        {
            var cpfCnpjEmitente = nfes.Select(x => new { CpfCnpj = x.infNFe.emit.CNPJ ?? x.infNFe.emit.CPF }).Distinct().SingleOrDefault();
            return !(cpfCnpjEmitente.CpfCnpj != cfgServico.Emitente.CpfCnpj);
        }

        private bool ValidarModeloUnico(IList<NFe> nfes)
        {
            var contar = nfes.Select(x => x.infNFe.ide.mod).Distinct().Count();
            return contar == 1;
        }

        public void Validar()
        {
            if (!ValidarEmitenteEmissor(nfes))
                throw new FalhaValidacaoException("CNPJ/CPF do Emitente da NFe difere do CNPJ/CPF do Emissor Cadastrado");

            if (!ValidarModeloUnico(nfes))
                throw new Exception("Apenas um modelo de NFe pode ser enviado por lote");
        }
    }
}
