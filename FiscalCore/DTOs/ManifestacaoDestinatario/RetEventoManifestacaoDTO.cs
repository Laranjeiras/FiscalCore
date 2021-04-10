using FiscalCore.Modelos.Retornos;
using FiscalCore.Tipos;
using FiscalCore.Utils;
using System;

namespace FiscalCore.DTOs.ManifestacaoDestinatario
{
    public class RetEventoManifestacaoDTO
    {
        public RetEventoManifestacaoDTO(infEventoRet infEventoRet)
        {
            if (infEventoRet == null)
                throw new ArgumentNullException(nameof(infEventoRet));

            Id = infEventoRet.Id;
            TipoAmbiente = infEventoRet.tpAmb;
            VerAplic = infEventoRet.verAplic;
            COrgao = infEventoRet.cOrgao;
            CStat = infEventoRet.cStat;
            Motivo = infEventoRet.xMotivo;
            ChaveNFe = infEventoRet.chNFe;
            TipoEvento = infEventoRet.tpEvento;
            XEvento = infEventoRet.xEvento;
            NSeqEvento = infEventoRet.nSeqEvento;
            CpfCnpjDest = infEventoRet.CnpjDest ?? infEventoRet.CpfDest;
            DhRegEvento = infEventoRet.dhRegEvento;
            NProt = infEventoRet.nProt;
            Xml = XmlUtils.ClasseParaXmlString<infEventoRet>(infEventoRet);
        }

        public string Id { get; protected set; }
        public eTipoAmbiente TipoAmbiente { get; protected set; }
        public string VerAplic { get; protected set; }
        public eUF COrgao { get; protected set; }
        public int CStat { get; protected set; }
        public string Motivo { get; protected set; }
        public string ChaveNFe { get; protected set; }
        public eTipoEventoNFe? TipoEvento { get; protected set; }
        public string XEvento { get; protected set; }
        public int? NSeqEvento { get; protected set; }
        public string CpfCnpjDest { get; protected set; }
        public DateTime DhRegEvento { get; protected set; }
        public string NProt { get; protected set; }

        public string Xml { get; protected set; }
        public bool Registrado => CStat == 135 || CStat == 136;
    }
}
