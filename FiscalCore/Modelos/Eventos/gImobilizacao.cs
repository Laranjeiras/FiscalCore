#nullable disable
#pragma warning disable CS8981
using System.Globalization;
using System.Xml.Serialization;

namespace FiscalCore.Modelos.Eventos
{
    public class gImobilizacao
    {
        [XmlAttribute]
        public int nItem { get; set; }

        [XmlIgnore]
        public decimal vIBS { get; set; }

        [XmlElement("vIBS")]
        public string ProxyvIBS
        {
            get => vIBS.ToString("0.00", CultureInfo.InvariantCulture);
            set => vIBS = decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        [XmlIgnore]
        public decimal vCBS { get; set; }

        [XmlElement("vCBS")]
        public string ProxyvCBS
        {
            get => vCBS.ToString("0.00", CultureInfo.InvariantCulture);
            set => vCBS = decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        public gControleEstoque gControleEstoque { get; set; }
    }

    public class gControleEstoque
    {
        [XmlIgnore]
        public decimal qImobilizado { get; set; }

        [XmlElement("qImobilizado")]
        public string ProxyqImobilizado
        {
            get => qImobilizado.ToString("0.0000", CultureInfo.InvariantCulture);
            set => qImobilizado = decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        public string uImobilizado { get; set; }
    }
}
