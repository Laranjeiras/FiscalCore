using System;

namespace FiscalCore.ValueObjects
{
    /// <summary>
    /// CNPJ numérico ou alfanumérico, conforme IN RFB nº 2.119/2022 (Anexo XV).
    /// Posições 1–12 aceitam [A-Z0-9]; posições 13–14 (dígitos verificadores) são
    /// sempre numéricas. CNPJs numéricos legados permanecem válidos.
    /// </summary>
    public struct Cnpj
    {
        private const int TamanhoBase = 12;
        private const int Tamanho = 14;

        private readonly string _value;
        private readonly string _normalizado;

        public readonly bool Valido;

        /// <summary>Indica se o CNPJ contém letras (inscrição alfanumérica).</summary>
        public readonly bool Alfanumerico;

        static readonly int[] Multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        static readonly int[] Multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        public Cnpj(string value)
        {
            _value = value;
            _normalizado = Normalizar(value);
            Alfanumerico = ContemLetra(_normalizado);
            Valido = Validar(_normalizado);
        }

        /// <summary>CNPJ sem máscara e em maiúsculas. Formato de persistência.</summary>
        public string Value => _normalizado;

        public static bool IsValid(string? value) => Validar(Normalizar(value));

        /// <summary>
        /// Remove máscara e normaliza para maiúsculas. Preserva caracteres inválidos
        /// para que <see cref="Validar"/> os rejeite, em vez de descartá-los em silêncio.
        /// </summary>
        private static string Normalizar(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // A entrada chega de XML e de certificado, fora do controle desta camada:
            // o buffer é dimensionado pelo tamanho útil, nunca pelo da entrada.
            Span<char> buffer = stackalloc char[Tamanho];
            var tamanho = 0;

            foreach (var c in value)
            {
                if (c == '.' || c == '/' || c == '-' || c == ' ')
                    continue;

                if (tamanho == Tamanho)
                    return string.Empty; // excede o comprimento de um CNPJ

                buffer[tamanho++] = char.ToUpperInvariant(c);
            }

            return new string(buffer[..tamanho]);
        }

        private static bool ContemLetra(string cnpj)
        {
            foreach (var c in cnpj)
            {
                if (c >= 'A' && c <= 'Z')
                    return true;
            }

            return false;
        }

        private static bool Validar(string cnpj)
        {
            if (cnpj.Length != Tamanho)
                return false;

            // Posições 1–12: [A-Z0-9]. Posições 13–14: apenas dígitos.
            for (var i = 0; i < TamanhoBase; i++)
            {
                if (!EhAlfanumerico(cnpj[i]))
                    return false;
            }

            if (!char.IsDigit(cnpj[12]) || !char.IsDigit(cnpj[13]))
                return false;

            if (SequenciaHomogenea(cnpj))
                return false;

            var totalDigito1 = 0;
            var totalDigito2 = 0;

            for (var i = 0; i < TamanhoBase; i++)
            {
                var valor = ValorNumerico(cnpj[i]);
                totalDigito1 += valor * Multiplicador1[i];
                totalDigito2 += valor * Multiplicador2[i];
            }

            var dv1 = CalcularDigito(totalDigito1);
            if (cnpj[12] - '0' != dv1)
                return false;

            totalDigito2 += dv1 * Multiplicador2[TamanhoBase];

            return cnpj[13] - '0' == CalcularDigito(totalDigito2);
        }

        private static bool EhAlfanumerico(char c)
            => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z');

        /// <summary>Valor do caractere na tabela da RFB: ASCII − 48 ('0'→0, 'A'→17).</summary>
        private static int ValorNumerico(char c) => c - '0';

        private static int CalcularDigito(int total)
        {
            var resto = total % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        /// <summary>
        /// Rejeita raiz repetida (ex.: "00000000000000", "AAAAAAAAAAAA45"). A verificação
        /// cobre as 12 primeiras posições: os DVs são derivados da base e não devem
        /// mascarar a repetição.
        /// </summary>
        private static bool SequenciaHomogenea(string cnpj)
        {
            for (var i = 1; i < TamanhoBase; i++)
            {
                if (cnpj[i] != cnpj[0])
                    return false;
            }

            return true;
        }

        public static implicit operator Cnpj(string value)
            => new Cnpj(value);

        public override string ToString()
            => _value;

        public static bool ValidarCNPJ(Cnpj cnpj)
            => cnpj.Valido;
    }
}
