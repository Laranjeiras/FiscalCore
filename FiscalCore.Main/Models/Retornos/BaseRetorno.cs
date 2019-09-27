namespace FiscalCore.Main.Models.Retornos
{
    public abstract class BaseRetorno
    {
        public BaseRetorno(string xmlRetStr)
        {
            XmlRetStr = xmlRetStr;
        }

        public string XmlRetStr { get; set; }
    }
}