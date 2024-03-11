using FiscalCore.Configuracoes;
using FiscalCore.Exceptions;
using FiscalCore.NotaFiscal;
using FiscalCore.Tipos;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Servicos
{
    internal static class ValidarNFeAutorizacao
    {
        public static void ValidarPoliticas(IList<NFe> nfes, ConfiguracaoBasicaServico configuracao)
        {
            if (!ValidarEmitenteEmissor(nfes, configuracao.CNPJEmitente))
                throw new FalhaValidacaoException("CNPJ/CPF do Emitente da NFe difere do CNPJ/CPF do Emissor Cadastrado");

            if (!ModeloUnico(nfes))
                throw new FalhaValidacaoException("Apenas um modelo de NFe pode ser enviado por lote");

            foreach (NFe nfe in nfes)
            {
                if (nfe.infNFe.ide.mod == eModeloDocumento.NFCe)
                    ValidarPresencaoCompradorNFCe(nfe);
            }
        }

        private static bool ValidarEmitenteEmissor(IList<NFe> nfes, string cnpjEmissor)
        {
            try
            {
                var cpfCnpjEmitente = nfes
                    .Select(x => new { CpfCnpj = x.infNFe.emit.CNPJ ?? x.infNFe.emit.CPF })
                    .Distinct()
                    .SingleOrDefault();
                return !(cpfCnpjEmitente.CpfCnpj != cnpjEmissor);
            }
            catch
            {
                return false;
            }
        }

        private static bool ModeloUnico(IList<NFe> nfes)
        {
            var contar = nfes
                .Select(x => x.infNFe.ide.mod)
                .Distinct()
                .Count();

            return contar == 1;
        }

        private static void ValidarPresencaoCompradorNFCe(NFe nfe)
        {
            if (nfe.infNFe.ide.indPres != ePresencaComprador.Presencial)
                throw new FalhaValidacaoException("NFC-e em operacao nao presencial");
        }
    }
}
