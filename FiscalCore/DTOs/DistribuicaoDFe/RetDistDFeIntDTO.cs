using System;
using FiscalCore.Extensions;

namespace FiscalCore.DTOs.DistribuicaoDFe
{
    public class RetDistDFeIntDTO
    {

        public RetDistDFeIntDTO(string chaveNFe, long nSU, string xml, string tipoRetorno)
        {
            ChaveNFe = chaveNFe;
            NSU = nSU;
            Xml = xml;
            TipoRetorno = tipoRetorno;
        }

        public Guid Id { get; protected set; } = Guid.NewGuid();

        private string _chaveNFe;

        public string ChaveNFe
        {
            protected set { 

                _chaveNFe = value.SomenteNumeros(); 
            }
            get { return _chaveNFe; }
        }

        public long NSU { get; protected set; }
        public string Xml { get; protected set; }
        public string TipoRetorno { get; protected set; }
    }
}
