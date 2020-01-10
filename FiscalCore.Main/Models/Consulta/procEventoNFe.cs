using FiscalCore.Main.Models.Eventos;
using FiscalCore.Main.Models.Retornos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using retEvento = FiscalCore.Main.Models.Retornos.retEvento;

namespace FiscalCore.Main.Models.Consulta
{
    [XmlRoot(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class procEventoNFe
    {
        /// <summary>
        ///     ZR02
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     ZR03
        /// </summary>
        public evento evento { get; set; }

        /// <summary>
        ///     YR05
        /// </summary>
        public retEvento retEvento { get; set; }
    }
}
