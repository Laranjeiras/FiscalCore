#nullable disable
#pragma warning disable CS8981
using FiscalCore.Utils;

namespace FiscalCore.NotaFiscal.Informacoes.Total
{
    /// <summary>
    ///     W34 - Totais da NF-e com IBS e CBS (TIBSCBSMonoTot, PL 010f).
    ///     Monofasia (gMono) e estorno de crédito (gEstornoCred) entram por adição.
    /// </summary>
    public class IBSCBSTot
    {
        /// <summary>
        ///     Valor total da base de cálculo do IBS e da CBS
        /// </summary>
        public decimal vBCIBSCBS
        {
            get { return _vBCIBSCBS.Arredondar(2); }
            set { _vBCIBSCBS = value.Arredondar(2); }
        }

        /// <summary>
        ///     Grupo total do IBS
        /// </summary>
        public gIBSTot gIBS { get; set; }

        /// <summary>
        ///     Grupo total da CBS
        /// </summary>
        public gCBSTot gCBS { get; set; }

        private decimal _vBCIBSCBS;
    }

    public class gIBSTot
    {
        public gIBSUFTot gIBSUF { get; set; }

        public gIBSMunTot gIBSMun { get; set; }

        public decimal vIBS
        {
            get { return _vIBS.Arredondar(2); }
            set { _vIBS = value.Arredondar(2); }
        }

        public decimal vCredPres
        {
            get { return _vCredPres.Arredondar(2); }
            set { _vCredPres = value.Arredondar(2); }
        }

        public decimal vCredPresCondSus
        {
            get { return _vCredPresCondSus.Arredondar(2); }
            set { _vCredPresCondSus = value.Arredondar(2); }
        }

        private decimal _vIBS;
        private decimal _vCredPres;
        private decimal _vCredPresCondSus;
    }

    public class gIBSUFTot
    {
        public decimal vDif
        {
            get { return _vDif.Arredondar(2); }
            set { _vDif = value.Arredondar(2); }
        }

        public decimal vDevTrib
        {
            get { return _vDevTrib.Arredondar(2); }
            set { _vDevTrib = value.Arredondar(2); }
        }

        public decimal vIBSUF
        {
            get { return _vIBSUF.Arredondar(2); }
            set { _vIBSUF = value.Arredondar(2); }
        }

        private decimal _vDif;
        private decimal _vDevTrib;
        private decimal _vIBSUF;
    }

    public class gIBSMunTot
    {
        public decimal vDif
        {
            get { return _vDif.Arredondar(2); }
            set { _vDif = value.Arredondar(2); }
        }

        public decimal vDevTrib
        {
            get { return _vDevTrib.Arredondar(2); }
            set { _vDevTrib = value.Arredondar(2); }
        }

        public decimal vIBSMun
        {
            get { return _vIBSMun.Arredondar(2); }
            set { _vIBSMun = value.Arredondar(2); }
        }

        private decimal _vDif;
        private decimal _vDevTrib;
        private decimal _vIBSMun;
    }

    public class gCBSTot
    {
        public decimal vDif
        {
            get { return _vDif.Arredondar(2); }
            set { _vDif = value.Arredondar(2); }
        }

        public decimal vDevTrib
        {
            get { return _vDevTrib.Arredondar(2); }
            set { _vDevTrib = value.Arredondar(2); }
        }

        public decimal vCBS
        {
            get { return _vCBS.Arredondar(2); }
            set { _vCBS = value.Arredondar(2); }
        }

        public decimal vCredPres
        {
            get { return _vCredPres.Arredondar(2); }
            set { _vCredPres = value.Arredondar(2); }
        }

        public decimal vCredPresCondSus
        {
            get { return _vCredPresCondSus.Arredondar(2); }
            set { _vCredPresCondSus = value.Arredondar(2); }
        }

        private decimal _vDif;
        private decimal _vDevTrib;
        private decimal _vCBS;
        private decimal _vCredPres;
        private decimal _vCredPresCondSus;
    }
}
