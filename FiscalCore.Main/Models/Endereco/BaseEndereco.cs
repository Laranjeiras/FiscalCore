using FiscalCore.Main.Enums;

namespace FiscalCore.Main.Models.Endereco
{
    public class BaseEndereco
    {
        public BaseEndereco()
        {

        }

        public BaseEndereco(string xLgr, string nro, string xCpl, string xBairro, long cMun, string xMun, eUF uF, string cep, int? cPais, string xPais, long? fone)
        {
            this.xLgr = xLgr;
            this.nro = nro;
            this.xCpl = xCpl;
            this.xBairro = xBairro;
            this.cMun = cMun;
            this.xMun = xMun;
            this.UF = uF;
            this.CEP = cep;
            this.cPais = cPais;
            this.xPais = xPais;
            this.fone = fone;
        }

        public string xLgr { get; set; }
        public string nro { get; set; }
        public string xCpl { get; set; }
        public string xBairro { get; set; }
        public long cMun { get; set; }
        public string xMun { get; set; }
        public eUF UF { get; set; }
        public string CEP { get; set; }
        public int? cPais { get; protected set; } = 1058;
        public string xPais { get; protected set; } = "Brasil";
        public long? fone { get; set; }
    }
}
