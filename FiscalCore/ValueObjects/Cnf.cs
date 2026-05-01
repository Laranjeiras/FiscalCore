using FiscalCore.Exceptions;
using System;

namespace FiscalCore.ValueObjects
{
    public class Cnf
    {
        public Cnf(int codigo)
        {
            if (!Validar(codigo.ToString()))
                throw new FalhaValidacaoException("cNF inválido");

            value = codigo.ToString();
        }

        public Cnf(string codigo)
        {
            if (!Validar(codigo))
                throw new FalhaValidacaoException("cNF inválido");

            value = Formatar(codigo);
        }

        private string value;
        public string Value => Formatar(value);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cnf);
        }

        public bool Equals(Cnf? other)
        {
            return !(other is null) &&
                   Value == other.Value &&
                   ToString() == other.ToString();
        }

        public override string ToString()
        {
            return Value;
        }

        private string Formatar(string cnf)
        {
            return cnf.PadLeft(8, '0');
        }

        private bool Validar(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return false;

            if (codigo.Length <= 0 || codigo.Length > 8)
                return false;

            var n = int.TryParse(codigo, out int result);
            if (!n)
                return false;
                
            if (result < 0 || result > 999999999)
                return false;

            return true;
        }
    }
}
