using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FiscalCore.Utils
{
    public class Notificacao
    {
        public string Campo { get; protected set; }
        public string Mensagem { get; protected set; }
        public bool EhErro { get; protected set; }

        public Notificacao(string campo, string mensagem, bool ehErro = true)
        {
            Campo = campo;
            Mensagem = mensagem;
            EhErro = ehErro;
        }
    }

    public class Contrato
    {
        public Contrato()
        {
            Notificacoes = new List<Notificacao>();
        }

        public ICollection<Notificacao> Notificacoes { get; private set; }
        public bool Valido => Notificacoes.Any(x => x.EhErro);

        public void Add(Notificacao notificacao)
        {
            Notificacoes.Add(notificacao);
        }
    }
}
