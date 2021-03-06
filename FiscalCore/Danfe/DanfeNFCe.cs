﻿using FiscalCore.Danfe.NFCe;
using FiscalCore.Configuracoes;

namespace FiscalCore.Danfe
{
    public class DanfeNFCe
    {
        private readonly DanfeNativoNfce _danfe;

        public DanfeNFCe(string xml, IConfiguracaoDanfe configDanfe, string cIdtoken, string csc)
        {
            _danfe = new DanfeNativoNfce(xml, configDanfe, cIdtoken, csc);
        }

        public void SalvarJpg(string filename)
        {
            _danfe.SalvarJpg(filename);
        }

        public byte[] ObterBytesJpg()
        {
            return _danfe.ObterBytesJpg();
        }

        public void Imprimir(string nomeImpressora)
        {
            _danfe.Imprimir(nomeImpressora);
        }

    }
}
