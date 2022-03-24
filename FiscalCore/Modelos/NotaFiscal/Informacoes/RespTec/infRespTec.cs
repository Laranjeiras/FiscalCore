using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.Informacoes.RespTec
{
    public class infRespTec
    {
        /// <summary>
        ///    ZD02 - Informar o CNPJ da pessoa jurídica responsável pelo sistema utilizado na emissão do documento fiscal eletrônico.
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///     ZD04 - Informar o nome da pessoa a ser contatada na empresa desenvolvedora do sistema utilizado na emissão do documento fiscal eletrônico.
        /// </summary>
        public string xContato { get; set; }

        /// <summary>
        ///     ZD05 - Informar o e-mail da pessoa a ser contatada na empresa desenvolvedora do sistema.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        ///     ZD06 - Informar o telefone da pessoa a ser contatada na empresa desenvolvedora do sistema. Preencher com o Código DDD + número do telefone.
        /// </summary>
        public string fone { get; set; }

        /// <summary>
        ///     ZD08 - Identificador do CSRT utilizado para montar o hash do CSRT
        /// </summary>
        [XmlIgnore]
        public int? idCSRT { get; set; }


        [XmlElement(ElementName = "idCSRT")]
        public string ProxyidCSRT
        {
            get
            {
                if (idCSRT == null) return null;


                return idCSRT.Value.ToString("D2");
            }
            set
            {
                if (value == null)
                {
                    idCSRT = null;
                    return;
                }
                idCSRT = int.Parse(value);
            }
        }

        /// <summary>
        ///     ZD09 - Hash do CSRT
        /// </summary>
        public string hashCSRT { get; set; }
    }
}
