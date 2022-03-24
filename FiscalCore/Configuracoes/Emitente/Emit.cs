using FiscalCore.Tipos;

namespace FiscalCore.Configuracoes.Emitente
{
    public class emit
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string CpfCnpj => CNPJ ?? CPF;
        public string xNome { get; set; }
        public string xFant { get; set; }
        
        // <sumary>
        // Informar:
        //- Inscrição Estadual do transportador contribuinte do ICMS, sem caracteres de formatação(ponto, barra, hífen, etc.);
        //- Literal “ISENTO” para transportador isento de inscrição no cadastro de contribuintes ICMS;
        //- Não informar a tag para não contribuinte do ICMS,
        //A UF deve ser informada se informado uma IE. (v2.0)
        // </sumary>
        public string IE { get; set; }

        /// <summary>
        /// IE do Substituto Tributário da UF de destino da mercadoria, quando houver a retenção do ICMS ST para a UF de destino.
        /// </summary>
        public string IEST { get; set; }

        public string IM { get; set; }
        public string CNAE { get; set; }
        public eCRT CRT { get; set; }

        public enderEmit enderEmit { get; set; }
    }
}
