using FiscalCore.Extensions;
using FiscalCore.Tipos;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiscalCore.NotaFiscal.Protocolo;

public sealed record MensagemSefaz(string Codigo, string Mensagem);

[XmlType(IncludeInSchema = false)]
public enum TipoElementoMensagemSefaz
{
    [XmlEnum("cMsg")]
    Codigo,

    [XmlEnum("xMsg")]
    Mensagem
}

public class infProt
{
    private const int LimiteMensagensSefaz = 5;

    /// <summary>PR05 - Identificação do Ambiente.</summary>
    public eTipoAmbiente tpAmb { get; set; }

    /// <summary>PR06 - Versão do Aplicativo que processou a consulta.</summary>
    public string verAplic { get; set; } = string.Empty;

    /// <summary>PR07 - Chave de Acesso da NF-e.</summary>
    public string chNFe { get; set; } = string.Empty;

    /// <summary>PR08 - Data e hora de recebimento.</summary>
    [XmlIgnore]
    public DateTimeOffset dhRecbto { get; set; }

    [XmlElement(ElementName = "dhRecbto")]
    public string ProxyDhRecbto
    {
        get => dhRecbto.ParaDataHoraStringUtc()!;
        set => dhRecbto = DateTimeOffset.Parse(value);
    }

    /// <summary>PR09 - Número do Protocolo da NF-e.</summary>
    public string? nProt { get; set; }

    /// <summary>PR10 - Digest Value da NF-e processada.</summary>
    public string? digVal { get; set; }

    /// <summary>PR11 - Código do status da resposta.</summary>
    public int cStat { get; set; }

    /// <summary>PR12 - Descrição literal do status da resposta.</summary>
    public string xMotivo { get; set; } = string.Empty;

    /// <summary>PR13-PR15 - Sequência XML de até cinco pares cMsg/xMsg.</summary>
    [XmlElement("cMsg", typeof(string))]
    [XmlElement("xMsg", typeof(string))]
    [XmlChoiceIdentifier(nameof(TiposElementosMensagemSefaz))]
    public string[] ElementosMensagemSefaz { get; set; } = [];

    [XmlIgnore]
    public TipoElementoMensagemSefaz[] TiposElementosMensagemSefaz { get; set; } = [];

    [XmlIgnore]
    public IReadOnlyList<MensagemSefaz> MensagensSefaz
    {
        get
        {
            var mensagens = new List<MensagemSefaz>(LimiteMensagensSefaz);
            var elementosMensagemSefaz = ElementosMensagemSefaz ?? [];
            var tiposElementosMensagemSefaz = TiposElementosMensagemSefaz ?? [];

            for (var indice = 0;
                 indice + 1 < elementosMensagemSefaz.Length && mensagens.Count < LimiteMensagensSefaz;
                 indice++)
            {
                if (tiposElementosMensagemSefaz.Length <= indice + 1
                    || tiposElementosMensagemSefaz[indice] != TipoElementoMensagemSefaz.Codigo
                    || tiposElementosMensagemSefaz[indice + 1] != TipoElementoMensagemSefaz.Mensagem)
                {
                    continue;
                }

                mensagens.Add(new MensagemSefaz(
                    elementosMensagemSefaz[indice],
                    elementosMensagemSefaz[indice + 1]));
                indice++;
            }

            return mensagens;
        }
    }
}
