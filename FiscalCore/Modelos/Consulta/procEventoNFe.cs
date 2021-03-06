using FiscalCore.Modelos.Eventos;
using FiscalCore.Modelos.Retornos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using retEvento = FiscalCore.Modelos.Retornos.retEvento;

namespace FiscalCore.Modelos.Consulta
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
