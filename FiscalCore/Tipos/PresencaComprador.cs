using System.ComponentModel;

namespace FiscalCore.Tipos
{
    public enum ePresencaComprador
    {
        [Description("Não se aplica")]
        NaoSeAplica = 0,
        [Description("Operação presencial")]
        Presencial = 1,
        [Description("Operação não presencial, pela Internet")]
        Internet = 2,
        [Description("Operação não presencial, Teleatendimento")]
        Teleatendimento = 3,
        [Description("NFC-e em operação com entrega a domicílio")]
        EntregaDomicilio = 4,
        [Description("Fora do estabelecimento")]
        ForaEstabelecimento = 5,
        [Description("Operação não presencial, outros.")]
        NaoPresencialOutros = 9
    }
}
