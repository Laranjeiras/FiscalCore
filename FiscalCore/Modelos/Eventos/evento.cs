using System;
using System.Xml.Serialization;
using FiscalCore.Configuracoes.Emitente;
using FiscalCore.Modelos.Signatures;
using FiscalCore.Tipos;
using FiscalCore.ValueObjects;

namespace FiscalCore.Modelos.Eventos
{
    [XmlType(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class evento
    {
        /// <summary>
        ///     HP05 - Versão do leiaute do evento
        /// </summary>
        [XmlAttribute]
        public string versao { get; set; }

        /// <summary>
        ///     HP06 - Grupo de informações do registro do Evento
        /// </summary>
        public infEventoEnv infEvento { get; set; }

        /// <summary>
        ///     HP22 - Assinatura Digital do documento XML, a assinatura deverá ser aplicada no elemento infEvento
        /// </summary>
        [XmlElement(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }

        public static evento CriarEventoCancelamento(eUF uf, eTipoAmbiente tipoAmbiente, emit emitente, ChaveFiscal chave, string protocolo, string just, string versao)
        {
            var infEvento = new infEventoEnv
            {
                cOrgao = uf,
                tpAmb = tipoAmbiente,
                chNFe = chave.Chave,
                dhEvento = DateTime.Now,
                tpEvento = eTipoEventoNFe.NFeCancelamento,
                nSeqEvento = 1,
                verEvento = versao,
                detEvento = new detEvento
                {
                    nProt = protocolo,
                    versao = versao,
                    xJust = just,
                    descEvento = "Cancelamento"
                }
            };

            if (!string.IsNullOrEmpty(emitente.CNPJ))
                infEvento.CNPJ = emitente.CNPJ;
            else
                infEvento.CPF = emitente.CPF;

            var evento = new evento { versao = versao, infEvento = infEvento };

            return evento;
        }
    }
}
