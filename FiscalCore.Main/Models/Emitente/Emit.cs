﻿using FiscalCore.Main.Enums;

namespace FiscalCore.Main.Models.Emitente
{
    public class emit
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }
        public enderEmit enderEmit { get; set; }
        public string IE { get; set; }
        public string IEST { get; set; }
        public string IM { get; set; }
        public string CNAE { get; set; }
        public eCRT CRT { get; set; }
    }
}
