using FiscalCore.Exceptions;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;
using System;
using System.IO;

namespace FiscalCore.Configuracoes
{
    public class ConfiguracaoBasicaServico
    {
        public ConfiguracaoCertificado ConfigCertificado { get; set; } = null!;
        public string CNPJEmitente { get; set; } = null!;
        public eTipoAmbiente TipoAmbiente { get; set; } = eTipoAmbiente.Homologacao;
        public eUF UF { get; set; } = eUF.RJ;
        public int TimeOut { get; set; } = 5000;

        /// <summary>
        /// Ignora erro caso de erro ao salvar arquivos xml no storage
        /// </summary>
        public bool IgnorarErroDeStorage { get; set; }

        private string? _diretorioSchemas;
        public string DiretorioSchemas
        {
            get { return _diretorioSchemas ?? Directory.GetCurrentDirectory(); }
            set { _diretorioSchemas = value; }
        }

        /// <summary>
        /// Validar Xml com Schema
        /// </summary>
        public bool ValidarXmlSchema { get; set; }

        /// <summary>
        /// O emitente pode ser pessoa jurídica (CNPJ, inclusive alfanumérico) ou
        /// física (CPF) — <see cref="Emitente.emit.CpfCnpj"/> resolve para
        /// <c>CNPJ ?? CPF</c>, então ambos são aceitos aqui.
        /// </summary>
        public void Validar()
        {
            if (!DocumentoEmitenteValido())
            {
                throw new ConfiguracaoException($"{nameof(CNPJEmitente)} inválido");
            }
        }

        private bool DocumentoEmitenteValido()
        {
            if (string.IsNullOrWhiteSpace(CNPJEmitente))
                return false;

            return Cnpj.IsValid(CNPJEmitente) || CpfValido(CNPJEmitente);
        }

        private static bool CpfValido(string valor)
        {
            Span<char> digitos = stackalloc char[LayoutFiscal.TamanhoCpf];
            var tamanho = 0;

            foreach (var c in valor)
            {
                if (!char.IsDigit(c))
                {
                    if (c == '.' || c == '-' || c == ' ')
                        continue;

                    return false;
                }

                if (tamanho == LayoutFiscal.TamanhoCpf)
                    return false;

                digitos[tamanho++] = c;
            }

            if (tamanho != LayoutFiscal.TamanhoCpf)
                return false;

            var homogeneo = true;
            for (var i = 1; i < LayoutFiscal.TamanhoCpf && homogeneo; i++)
            {
                homogeneo = digitos[i] == digitos[0];
            }

            if (homogeneo)
                return false;

            return DigitoCpf(digitos, 9) == digitos[9] - '0'
                && DigitoCpf(digitos, 10) == digitos[10] - '0';
        }

        /// <summary>Módulo 11 do CPF: pesos decrescentes a partir de <c>tamanho + 1</c>.</summary>
        private static int DigitoCpf(ReadOnlySpan<char> digitos, int tamanho)
        {
            var soma = 0;
            for (var i = 0; i < tamanho; i++)
            {
                soma += (digitos[i] - '0') * (tamanho + 1 - i);
            }

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
