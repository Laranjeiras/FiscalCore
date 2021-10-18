using FiscalCore.Tipos;
using System;
using System.Linq;
using System.Text;

namespace FiscalCore.ValueObjects
{
    public class ChaveFiscal
    {
        private string chave;
        public string Chave => chave;

        private byte digitoVerificador;
        public byte DigitoVerificador => digitoVerificador;

        public string Completa => ModeloToString + Chave;

        private string ModeloToString =>
            Modelo == eModeloDocumento.NFCe || Modelo == eModeloDocumento.NFe ? "NFe" : throw new ArgumentOutOfRangeException("Modelo documento");

        public eUF UF { get; private set; }
        public string AnoMesEmissao { get; private set; }
        public string Cnpj { get; private set; }
        public eModeloDocumento Modelo { get; private set; }
        public int Serie { get; private set; }
        public long Numero { get; private set; }
        public eTipoEmissao TipoEmissao { get; private set; }
        public string CNF { get; private set; }

        public ChaveFiscal(eUF uf, DateTime dataEmissao, string cnpj, eModeloDocumento modelo, int serie, long numero, eTipoEmissao tipoEmissao, string cNF)
        {
            this.UF = uf;
            this.AnoMesEmissao = dataEmissao.ToString("yyMM");
            this.Cnpj = cnpj;
            this.Modelo = modelo;
            this.Serie = serie;
            this.Numero = numero;
            this.TipoEmissao = tipoEmissao;
            this.CNF = cNF;
            this.chave = GerarChave();
        }

        public ChaveFiscal(string chave)
        {
            if (!(chave.Length == 44))
                throw new ArgumentOutOfRangeException(nameof(chave));

            chave = new string(chave.Where(Char.IsDigit).ToArray());
            
            this.UF = (eUF)Enum.Parse(typeof(eUF), chave.Substring(0, 2));
            this.AnoMesEmissao = chave.Substring(2, 4);
            this.Cnpj = chave.Substring(6, 14);
            this.Modelo = (eModeloDocumento)Enum.Parse(typeof(eModeloDocumento), chave.Substring(20, 2));
            this.Serie = int.Parse(chave.Substring(22, 3));
            this.Numero = long.Parse(chave.Substring(25, 9));
            this.TipoEmissao = (eTipoEmissao)Enum.Parse(typeof(eTipoEmissao), chave.Substring(34, 1));
            this.CNF = chave.Substring(35, 8);
            this.digitoVerificador = byte.Parse(chave.Substring(43, 1));
            this.chave = GerarChave();
        }

        private string GerarChave()
        {
            var chave = new StringBuilder();
            chave.Append(((int)UF).ToString("D2"));
            chave.Append(AnoMesEmissao);
            chave.Append(Cnpj);
            chave.Append(((int)Modelo).ToString("D2"));
            chave.Append(Serie.ToString("D3"));
            chave.Append(Numero.ToString("D9"));
            chave.Append(((int)TipoEmissao).ToString());
            chave.Append(CNF.PadLeft(8 - CNF.Length, '0'));

            var digitoVerificador = CalcularDigitoVerificador(chave.ToString());
            this.digitoVerificador = byte.Parse(digitoVerificador);

            chave.Append(digitoVerificador);

            return chave.ToString();
        }

        /// <summary>
        /// (B-23) Dígito Verificador da Chave de Acesso da NF-e
        /// </summary>
        /// <param name="chave"></param>
        /// <returns></returns>
        private static string CalcularDigitoVerificador(string chave)
        {
            chave = new string(chave.Where(Char.IsDigit).ToArray());

            int soma = 0;
            int peso = 2;

            for (var i = chave.Length - 1; i != -1; i--)
            {
                var ch = Convert.ToInt32(chave[i].ToString());
                soma += ch * peso;
                peso = peso < 9 ? peso + 1 : 2;
            }

            int mod = soma % 11;
            int dv = (mod == 0 || mod == 1) ? 0 : 11 - mod;

            return dv.ToString();
        }

        public override string ToString()
        {
            return Completa.ToString();
        }
    }
}
