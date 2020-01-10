namespace FiscalCore.Main.Models.Retornos
{
    public abstract class BaseRetorno
    {
        public BaseRetorno()
        {

        }

        public BaseRetorno(string xmlRecebido)
        {
            XmlRecebido = xmlRecebido;
        }

        public string XmlRecebido { get; set; }

        public string XmlEnviado { get; set; }
    }
}