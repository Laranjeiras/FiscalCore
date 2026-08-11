using FiscalCore.NotaFiscal.RetornoServicos.Recepcao.Retorno;
using FiscalCore.Utils;
using Xunit;

namespace FiscalCore.Testes.NotaFiscal.Protocolo;

public sealed class InfProtTests
{
    [Fact]
    public void XmlStringParaClasse_ProtocoloComCincoAlertas_PreservaOrdemEConteudo()
    {
        const string xml = """
            <retConsReciNFe xmlns="http://www.portalfiscal.inf.br/nfe" versao="4.00">
              <tpAmb>2</tpAmb><verAplic>SVRS20260811</verAplic><nRec>123456789012345</nRec>
              <cStat>104</cStat><xMotivo>Lote processado</xMotivo><cUF>43</cUF>
              <dhRecbto>2026-08-11T10:30:00-03:00</dhRecbto>
              <protNFe versao="4.00"><infProt>
                <tpAmb>2</tpAmb><verAplic>SVRS20260811</verAplic>
                <chNFe>43260829310114000110550010000017401123456780</chNFe>
                <dhRecbto>2026-08-11T10:30:00-03:00</dhRecbto>
                <nProt>143260000000001</nProt><digVal>YWJjZGVmZ2hpamtsbW5vcHFyc3Q=</digVal>
                <cStat>120</cStat><xMotivo>Autorizado o uso da NF-e, com alerta</xMotivo>
                <cMsg>1001</cMsg><xMsg>Primeiro alerta</xMsg>
                <cMsg>1002</cMsg><xMsg>Segundo alerta</xMsg>
                <cMsg>1003</cMsg><xMsg>Terceiro alerta</xMsg>
                <cMsg>1004</cMsg><xMsg>Quarto alerta</xMsg>
                <cMsg>1005</cMsg><xMsg>Quinto alerta</xMsg>
              </infProt></protNFe>
            </retConsReciNFe>
            """;

        var retorno = XmlUtils.XmlStringParaClasse<retConsReciNFe>(xml);
        var alertas = Assert.Single(retorno.protNFe).infProt.MensagensSefaz;

        Assert.Collection(
            alertas,
            alerta => Assert.Equal(("1001", "Primeiro alerta"), (alerta.Codigo, alerta.Mensagem)),
            alerta => Assert.Equal(("1002", "Segundo alerta"), (alerta.Codigo, alerta.Mensagem)),
            alerta => Assert.Equal(("1003", "Terceiro alerta"), (alerta.Codigo, alerta.Mensagem)),
            alerta => Assert.Equal(("1004", "Quarto alerta"), (alerta.Codigo, alerta.Mensagem)),
            alerta => Assert.Equal(("1005", "Quinto alerta"), (alerta.Codigo, alerta.Mensagem)));
    }

    [Fact]
    public void ClasseParaXmlString_ProtocoloComAlertas_MantemParesIntercalados()
    {
        const string xml = """
            <retConsReciNFe xmlns="http://www.portalfiscal.inf.br/nfe" versao="4.00">
              <tpAmb>2</tpAmb><verAplic>SVRS</verAplic><nRec>1</nRec><cStat>104</cStat>
              <xMotivo>Lote processado</xMotivo><cUF>43</cUF>
              <dhRecbto>2026-08-11T10:30:00-03:00</dhRecbto>
              <protNFe versao="4.00"><infProt>
                <tpAmb>2</tpAmb><verAplic>SVRS</verAplic>
                <chNFe>43260829310114000110550010000017401123456780</chNFe>
                <dhRecbto>2026-08-11T10:30:00-03:00</dhRecbto>
                <nProt>143260000000001</nProt><cStat>120</cStat>
                <xMotivo>Autorizado com alerta</xMotivo>
                <cMsg>101</cMsg><xMsg>Alerta A</xMsg><cMsg>102</cMsg><xMsg>Alerta B</xMsg>
              </infProt></protNFe>
            </retConsReciNFe>
            """;

        var retorno = XmlUtils.XmlStringParaClasse<retConsReciNFe>(xml);
        var serializado = XmlUtils.ClasseParaXmlString(retorno);

        Assert.Contains(
            "<cMsg>101</cMsg><xMsg>Alerta A</xMsg><cMsg>102</cMsg><xMsg>Alerta B</xMsg>",
            serializado);
    }
}
