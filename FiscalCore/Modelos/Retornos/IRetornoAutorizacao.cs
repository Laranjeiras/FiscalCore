#nullable disable
#pragma warning disable CS8981
namespace FiscalCore.Modelos.Retornos
{
    public interface IRetornoAutorizacao
    {
        retEnviNFe Retorno { get; set; }
        bool Autorizado { get; }
    }
}
