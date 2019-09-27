# FiscalCore

Dotnet Standard 2.1

#Exemplo NFe Autorização

var cert = new ConfiguracaoCertificado(eTipoCertificado.A3, "3b6cc164ef2d4cf7ac1488ac895cbedf");
var emit = new emit
{
    CNPJ = "",
    IE = "",
    xNome = "",
    CRT = eCRT.SimplesNacional,
    enderEmit = new enderEmit
    {
        CEP = "",
        cMun = 0,
        fone = 0,
        nro = "",
        UF = eUF.RJ,
        xBairro = "",
        xCpl = "",
        xLgr = "",
        xMun = "",
    }
};

var csc = new ConfiguracaoCsc("000001", "1b9a6ef1-1e29-4485-b0f8-d51d00247090");

var cfgServico = new ConfiguracaoServico(eTipoAmbiente.Homologacao, eUF.RJ, cert, emit, csc);
cfgServico.DiretorioSalvarXml = @"C:\FISCALCORE\XML\HOMOLOGACAO";
cfgServico.DiretorioSchemas = @"C:\FISCALCOLRE\NFeSchemas\PL_009_V4_00_NT_2018_005_v1.20";

// xmlString = Xml da NFe assinada

var servico = new FiscalCore.Servicos.Servicos.NFeAutorizacao4(cfgServico);
var retorno = servico.Autorizar(xmlString, eModeloDocumento.NFe);