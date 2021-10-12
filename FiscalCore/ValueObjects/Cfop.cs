using System;
using System.Collections.Generic;
using System.Text;

namespace FiscalCore.ValueObjects
{
    public class Cfop
    {
        public Cfop(int codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public int Codigo { get; protected set; }
        public string Descricao { get; protected set; }

        public static IList<Cfop> GerarCfops()
        {
            return new List<Cfop>
            {
                new Cfop(5102, "Venda de mercadoria adquirida ou recebida de terceiros"),
                new Cfop(5405, "Venda de mercadoria adquirida/recebida de terceiros em operação com mercadoria sujeita ao regime de substituição tributária, na condição de contrib substituído"),
            };
        }
    }


}
