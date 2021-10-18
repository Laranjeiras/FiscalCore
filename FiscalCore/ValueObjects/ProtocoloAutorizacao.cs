using System;
using System.Linq;

namespace FiscalCore.ValueObjects
{
    public class ProtocoloAutorizacao
    {
        public string Protocolo { get; protected set; }

        public ProtocoloAutorizacao(string protocolo)
        {
            Assert(protocolo);
            this.Protocolo = protocolo;
        }

        public static bool Validar(string protocolo)
        {
            try
            {
                Assert(protocolo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void Assert(string protocolo)
        {
            protocolo = new string(protocolo.Where(Char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(protocolo))
                throw new Exception("Protocolo de autorização inválido");
            if (!(protocolo.Length == 15))
                throw new Exception("Protocolo de autorização inválido");
        }
    }
}
