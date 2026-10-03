#nullable disable
#pragma warning disable CS8981
using FiscalCore.Utils;

namespace FiscalCore.NotaFiscal.Informacoes.Detalhe.Tributacao.Rtc
{
    /// <summary>
    ///     UB12 - Grupo IBS/CBS do item (TTribNFe, PL 010f).
    ///     Cobre a tributação regular com redução opcional (gRed). Os demais grupos do leiaute
    ///     (diferimento, monofasia, crédito presumido, transferência de crédito etc.) entram por adição.
    /// </summary>
    public class IBSCBS
    {
        /// <summary>
        ///     UB13 - Código de Situação Tributária do IBS e da CBS
        /// </summary>
        public string CST { get; set; }

        /// <summary>
        ///     UB14 - Código de Classificação Tributária do IBS e da CBS
        /// </summary>
        public string cClassTrib { get; set; }

        /// <summary>
        ///     UB15 - Grupo de informações do IBS e da CBS
        /// </summary>
        public gIBSCBS gIBSCBS { get; set; }
    }

    public class gIBSCBS
    {
        /// <summary>
        ///     UB16 - Base de cálculo do IBS e da CBS
        /// </summary>
        public decimal vBC
        {
            get { return _vBC.Arredondar(2); }
            set { _vBC = value.Arredondar(2); }
        }

        /// <summary>
        ///     UB17 - Grupo de informações do IBS para a UF
        /// </summary>
        public gIBSUF gIBSUF { get; set; }

        /// <summary>
        ///     UB36 - Grupo de informações do IBS para o município
        /// </summary>
        public gIBSMun gIBSMun { get; set; }

        /// <summary>
        ///     UB54a - Valor do IBS (soma de vIBSUF e vIBSMun)
        /// </summary>
        public decimal vIBS
        {
            get { return _vIBS.Arredondar(2); }
            set { _vIBS = value.Arredondar(2); }
        }

        /// <summary>
        ///     UB55 - Grupo de informações da CBS
        /// </summary>
        public gCBS gCBS { get; set; }

        private decimal _vBC;
        private decimal _vIBS;
    }

    public class gIBSUF
    {
        /// <summary>
        ///     UB18 - Alíquota do IBS de competência das UF (em percentual)
        /// </summary>
        public decimal pIBSUF
        {
            get { return _pIBSUF.Arredondar(4); }
            set { _pIBSUF = value.Arredondar(4); }
        }

        /// <summary>
        ///     Grupo de informações da redução da alíquota
        /// </summary>
        public gRed gRed { get; set; }

        /// <summary>
        ///     UB35 - Valor do IBS de competência da UF
        /// </summary>
        public decimal vIBSUF
        {
            get { return _vIBSUF.Arredondar(2); }
            set { _vIBSUF = value.Arredondar(2); }
        }

        private decimal _pIBSUF;
        private decimal _vIBSUF;
    }

    public class gIBSMun
    {
        /// <summary>
        ///     UB37 - Alíquota do IBS de competência do município (em percentual)
        /// </summary>
        public decimal pIBSMun
        {
            get { return _pIBSMun.Arredondar(4); }
            set { _pIBSMun = value.Arredondar(4); }
        }

        /// <summary>
        ///     Grupo de informações da redução da alíquota
        /// </summary>
        public gRed gRed { get; set; }

        /// <summary>
        ///     UB54 - Valor do IBS de competência do município
        /// </summary>
        public decimal vIBSMun
        {
            get { return _vIBSMun.Arredondar(2); }
            set { _vIBSMun = value.Arredondar(2); }
        }

        private decimal _pIBSMun;
        private decimal _vIBSMun;
    }

    public class gCBS
    {
        /// <summary>
        ///     UB56 - Alíquota da CBS (em percentual)
        /// </summary>
        public decimal pCBS
        {
            get { return _pCBS.Arredondar(4); }
            set { _pCBS = value.Arredondar(4); }
        }

        /// <summary>
        ///     Grupo de informações da redução da alíquota
        /// </summary>
        public gRed gRed { get; set; }

        /// <summary>
        ///     UB67 - Valor da CBS
        /// </summary>
        public decimal vCBS
        {
            get { return _vCBS.Arredondar(2); }
            set { _vCBS = value.Arredondar(2); }
        }

        private decimal _pCBS;
        private decimal _vCBS;
    }

    /// <summary>
    ///     TRed - Redução de alíquota
    /// </summary>
    public class gRed
    {
        /// <summary>
        ///     Percentual da redução de alíquota
        /// </summary>
        public decimal pRedAliq
        {
            get { return _pRedAliq.Arredondar(4); }
            set { _pRedAliq = value.Arredondar(4); }
        }

        /// <summary>
        ///     Alíquota efetiva aplicada à base de cálculo
        /// </summary>
        public decimal pAliqEfet
        {
            get { return _pAliqEfet.Arredondar(4); }
            set { _pAliqEfet = value.Arredondar(4); }
        }

        private decimal _pRedAliq;
        private decimal _pAliqEfet;
    }
}
