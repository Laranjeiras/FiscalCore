using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.Informacoes.Destinatario;
using DFeBR.EmissorNFe.Dominio.NotaFiscalEletronica.Informacoes.Pagamento;
using FiscalCore.Main.Configuracoes;
using FiscalCore.Main.Danfe.NFCe.Nativo;
using FiscalCore.Main.Models.Emitente;
using FiscalCore.Main.Utils;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using Zion.Common2.Extensions;

namespace FiscalCore.Main.Danfe.NFCe
{
    internal class DanfeNativoNfce
    {
        private NFe _nfe;
        private nfeProc _proc;
        private readonly Image _logo;        
        private readonly string _cIdToken, _csc;
        private readonly IConfiguracaoDanfe _configDanfe;
        private const int larguraLinha = 284;
        private const int larguraLinhaMargemDireita = 277;
        private int y;
        private int x = 3;

        public DanfeNativoNfce(string xml, IConfiguracaoDanfe configDanfe, string cIdToken, string csc)
        {
            _cIdToken = cIdToken;
            _csc = csc;
            _configDanfe = configDanfe;
            AdicionarTexto.FontPadrao = configDanfe.CarregarFontePadraoNfceNativa();
            CarregarXml(xml);
        }

        private void CarregarXml(string xml)
        {
            try
            {
                _proc = FuncoesXml.XmlStringParaClasse<nfeProc>(xml);
                _nfe = _proc.NFe;
            }
            catch (Exception)
            {
                try
                {
                    _nfe = FuncoesXml.XmlStringParaClasse<NFe>(xml);
                }
                catch (Exception)
                {
                    throw new ApplicationException(
                        "[Xml inválido] Ocorreu um erro ao carregar o xml");
                }
            }
        }

        //Função para mandar imprimir na impressora padrão
        //Utilizado em AppDesktop
        public void Imprimir(string nomeImpressora = null, string salvarArquivoPdfEm = null)
        {
            PrintDocument printCupom = new PrintDocument();

            printCupom.PrinterSettings.PrinterName = !string.IsNullOrEmpty(nomeImpressora) ?
                    nomeImpressora : printCupom.PrinterSettings.PrinterName;

            if (!string.IsNullOrEmpty(salvarArquivoPdfEm))
            {
                printCupom.DefaultPageSettings.PrinterSettings.PrintToFile = true;
                printCupom.DefaultPageSettings.PrinterSettings.PrintFileName = salvarArquivoPdfEm;
                printCupom.PrintController = new StandardPrintController();
            }

            printCupom.DocumentName = $@"Cupom NFCe {_nfe.infNFe.ide.nNF}/{_nfe.infNFe.ide.serie}";
            printCupom.PrintPage += printCupom_PrintPage;
            printCupom.Print();
        }

        private void printCupom_PrintPage(object sender, PrintPageEventArgs e)
        {
            GerarNfCe(e.Graphics);
        }

        public void SalvarJpg(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            var bitmap = GerarBitmap();
            bitmap.Save(filename, ImageFormat.Jpeg);
        }
        
        public byte[] ObterBytesJpg()
        {
            var bitmap = GerarBitmap();

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        private Bitmap GerarBitmap()
        {
            // Feito esse de cima para poder pegar o tamanho real da mesma desenhando
            using (Bitmap bmp = new Bitmap(300, 70000))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    GerarNfCe(g);
                }
            }

            // Obtive o tamanho real na posição y agora vou fazer um com tamanho exato
            Bitmap bmpFinal = new Bitmap(300, y);
            using (Graphics g = Graphics.FromImage(bmpFinal))
            {
                g.Clear(Color.White);
                GerarNfCe(g);

                return bmpFinal;
            }
        }

        private void GerarNfCe(Graphics g)
        {
            DesenharCabecalho(g);
            y += 3;
            LinhaHorizontal(g, x, y, larguraLinha);

            DesenharTabelaItens(g);
            y += 3;
            LinhaHorizontal(g, x, y, larguraLinha);
            
            DesenharTotais(g);
            y += 5;
            LinhaHorizontal(g, x, y, larguraLinha);

            DesenharInfoConsultaSefaz(g);
            DesenharMensagemConsumidor(g);
            DesenharMensagemDadosNFCe(g);
            DesenharDadosAutorizacao(g);
            DesenharMensagemContingencia(g);
            DesenharQrCode(g);
            DesenharTributosIncidentes(g);

            DesenharObservacoes(g);
            y += 3;
            LinhaHorizontal(g, x, y, larguraLinha);
            
            DesenharCreditos(g);
            y += 3;
        }

        private void DesenharCabecalho(Graphics g)
        {
            int larguraLogo = 64;
            int tamanhoFonteTitulo = 8;

            y = 3;

            if (_logo != null)
                new RedimensionarImagem(new AdicionarImagem(g, _logo, x, y), 50, 24).Desenhar();

            if (_logo == null)
                larguraLogo = 0;

            y = MontarLinhaTitulo(g, MontarMensagemRazaoSocial(), tamanhoFonteTitulo, larguraLogo, x, y, larguraLinha);
            y = MontarLinhaTitulo(g, MontarMensagemCpfCnpjIE(), tamanhoFonteTitulo, larguraLogo, x, y, larguraLinha);

            string enderecoEmitente = MontarMensagemEnderecoEmitente();

            AdicionarTexto textoEndereco = new AdicionarTexto(g, enderecoEmitente, 7);
            AdicionarTextoCentralizado(textoEndereco);
            y += 3;

            const string mensagemDanfe = "Documento Auxiliar da Nota Fiscal de Consumidor Eletrônica";
            AdicionarTexto textoDanfeNFCe = new AdicionarTexto(g, mensagemDanfe, 8);
            AdicionarTextoCentralizado(textoDanfeNFCe);
        }

        private void DesenharTabelaItens(Graphics g)
        {
            y += 3;
            int iniX = x;

            CriaHeaderColuna("CÓDIGO", g, iniX, y);
            iniX += 50;

            AdicionarTexto colunaDescricaoHeader = CriaHeaderColuna("DESCRIÇÃO", g, iniX, y);
            y += colunaDescricaoHeader.Medida.Altura;

            CriaHeaderColuna("QTDE", g, iniX, y);
            iniX += 25;

            CriaHeaderColuna("UN", g, iniX, y);
            iniX += 25;

            CriaHeaderColuna("x", g, iniX, y);
            iniX += 20;

            AdicionarTexto colunaValorUnitarioHeader = CriaHeaderColuna("VALOR UNITÁRIO", g, iniX, y);
            iniX += 85;

            CriaHeaderColuna("=", g, iniX, y);
            iniX += 41;

            AdicionarTexto colunaTotalHeader = CriaHeaderColuna("TOTAL", g, iniX, y);
            y += colunaTotalHeader.Medida.Altura + 10;

            var dets = _nfe.infNFe.det;

            #region preencher itens
            foreach (var detalhe in dets)
            {
                AdicionarTexto codigo = new AdicionarTexto(g, detalhe.prod.cProd, 7);
                codigo.Desenhar(x, y);

                AdicionarTexto nome = new AdicionarTexto(g, detalhe.prod.xProd, 7);
                QuebraDeLinha quebraNome = new QuebraDeLinha(nome, new ComprimentoMaximo(227), nome.Medida.Largura);
                nome = quebraNome.DesenharComQuebras(g);
                nome.Desenhar(x + 50, y);
                y += nome.Medida.Altura;

                var ucom = detalhe.prod?.uCom.Length > 3 ? detalhe.prod.uCom.Substring(0, 3) : string.Empty;
                AdicionarTexto quantidade = new AdicionarTexto(g, $"{detalhe.prod.qCom.ToString("N4")}   {ucom}", 7);
                AdicionarTexto valorUnitario = new AdicionarTexto(g, detalhe.prod.vUnCom.ToString("C"), 7);
                AdicionarTexto vezesX = new AdicionarTexto(g, "x", 7);

                decimal detalheTotal = detalhe.prod.vProd;
                AdicionarTexto valorTotalProduto = new AdicionarTexto(g, detalheTotal.ToString("C"), 7);

                iniX = x + 50;
                quantidade.Desenhar(iniX, y);

                iniX += 70;
                vezesX.Desenhar(iniX, y);

                iniX += 10;
                int tituloColunaUnidadeLargura = colunaValorUnitarioHeader.Medida.Largura;
                valorUnitario.Desenhar((iniX + tituloColunaUnidadeLargura) - valorUnitario.Medida.Largura, y);

                iniX += 85;
                AdicionarTexto igualColuna = new AdicionarTexto(g, "=", 7);
                igualColuna.Desenhar(iniX, y);

                iniX += 41;
                int tituloColunaTotal = colunaTotalHeader.Medida.Largura;
                valorTotalProduto.Desenhar((iniX + tituloColunaTotal) - valorTotalProduto.Medida.Largura, y);

                y += quantidade.Medida.Altura;
                decimal valorDescontoItem = detalhe.prod.vDesc ?? 0.0m;

                if (valorDescontoItem > 0.0m)
                {
                    AdicionarTexto descontoColuna = new AdicionarTexto(g, "Desconto", 7);
                    descontoColuna.Desenhar(x + 50, y);

                    StringBuilder descontoItemTexto = new StringBuilder("-");
                    descontoItemTexto.Append(valorDescontoItem.ToString("N2"));
                    AdicionarTexto valorDescontoItemColuna = new AdicionarTexto(g, descontoItemTexto.ToString(), 7);
                    int valorDescontoItemColunaX = ((x + 246) + tituloColunaTotal) -
                                                    valorDescontoItemColuna.Medida.Largura;
                    valorDescontoItemColuna.Desenhar(valorDescontoItemColunaX, y);

                    y += descontoColuna.Medida.Altura;
                }

                decimal valorAcrescimoItem = detalhe.prod.vOutro ?? 0.0m;
                if (valorAcrescimoItem > 0.0m)
                {
                    AdicionarTexto acrescimoColuna = new AdicionarTexto(g, "Acréscimo", 7);
                    acrescimoColuna.Desenhar(x + 50, y);

                    StringBuilder acrescimoItemTexto = new StringBuilder("+");
                    acrescimoItemTexto.Append(valorAcrescimoItem.ToString("N2"));
                    AdicionarTexto valorAcrescimoItemColuna = new AdicionarTexto(g, acrescimoItemTexto.ToString(), 7);
                    int valorAcrescimoItemColunaX = ((x + 246) + tituloColunaTotal) -
                                                    valorAcrescimoItemColuna.Medida.Largura;
                    valorAcrescimoItemColuna.Desenhar(valorAcrescimoItemColunaX, y);

                    y += acrescimoColuna.Medida.Altura;
                }

                if (valorDescontoItem > 0.0m || valorAcrescimoItem > 0.0m)
                {
                    AdicionarTexto valorLiquidoTexto = new AdicionarTexto(g, "Valor Líquido", 7);
                    valorLiquidoTexto.Desenhar(x + 50, y);

                    AdicionarTexto valorLiquidoTotalTexto = new AdicionarTexto(g,
                        ((detalheTotal + valorAcrescimoItem) - valorDescontoItem).ToString("N2"), 7);
                    int valorLiquidoTotalTextoX = ((x + 246) + tituloColunaTotal) -
                                                    valorLiquidoTotalTexto.Medida.Largura;
                    valorLiquidoTotalTexto.Desenhar(valorLiquidoTotalTextoX, y);

                    y += valorLiquidoTexto.Medida.Altura;
                }
            }
            #endregion
        }

        private void DesenharTotais(Graphics g)
        {
            y += 3;
            var dets = _nfe.infNFe.det;

            AdicionarTexto textoQuantidadeTotalItens = new AdicionarTexto(g, "Qtde. total de itens", 7);
            textoQuantidadeTotalItens.Desenhar(x, y);

            AdicionarTexto qtdTotalItens = new AdicionarTexto(g, dets.Count.ToString(), 7);
            int qtdTotalItensX = (larguraLinhaMargemDireita - qtdTotalItens.Medida.Largura);
            qtdTotalItens.Desenhar(qtdTotalItensX, y);
            y += textoQuantidadeTotalItens.Medida.Altura;

            AdicionarTexto textoValorTotal = new AdicionarTexto(g, "Valor total", 8);
            textoValorTotal.Desenhar(x, y);

            decimal valorTotal = dets.Sum(prod => prod.prod.vProd);
            AdicionarTexto valorTotalTexto = new AdicionarTexto(g, valorTotal.ToString("C"), 9);
            int qtdValorTotalX = (larguraLinhaMargemDireita - valorTotalTexto.Medida.Largura);
            valorTotalTexto.Desenhar(qtdValorTotalX, y);
            y += textoValorTotal.Medida.Altura;

            decimal totalDesconto = dets.Sum(prod => prod.prod.vDesc) ?? 0.0m;
            decimal totalOutras = dets.Sum(prod => prod.prod.vOutro) ?? 0.0m;
            decimal valorTotalAPagar = valorTotal + totalOutras - totalDesconto;

            if (totalDesconto > 0)
            {
                AdicionarTexto textoDesconto = new AdicionarTexto(g, "Desconto", 7);
                textoDesconto.Desenhar(x, y);

                AdicionarTexto valorDesconto = new AdicionarTexto(g, totalDesconto.ToString("C"), 7);
                int valorDescontoX = (larguraLinhaMargemDireita - valorDesconto.Medida.Largura);
                valorDesconto.Desenhar(valorDescontoX, y);
                y += textoDesconto.Medida.Altura;

                AdicionarTexto textoValorAPagar = new AdicionarTexto(g, "Valor a Pagar", 7);
                textoValorAPagar.Desenhar(x, y);

                AdicionarTexto valorAPagar = new AdicionarTexto(g, valorTotalAPagar.ToString("C"), 8);
                int valorAPagarX = (larguraLinhaMargemDireita - valorAPagar.Medida.Largura);
                valorAPagar.Desenhar(valorAPagarX, y);
                y += textoValorAPagar.Medida.Altura + 2;
            }

            GerarPagamento(g);
        }

        private void GerarPagamento(Graphics g)
        {
            y += 3;

            AdicionarTexto tituloFormaPagamento = new AdicionarTexto(g, "FORMA PAGAMENTO", 7);
            tituloFormaPagamento.Desenhar(x, y);

            AdicionarTexto tituloValorPago = new AdicionarTexto(g, "VALOR PAGO", 7);
            int tituloValorPagoX = (larguraLinhaMargemDireita - tituloValorPago.Medida.Largura);
            tituloValorPago.Desenhar(tituloValorPagoX, y);
            y += tituloFormaPagamento.Medida.Altura;

            foreach (var pag in _nfe.infNFe.pag)
            {
                if (pag.detPag != null)
                    foreach (var detPag in pag.detPag)
                        DesenharFormaPagamento(x, larguraLinhaMargemDireita, g, detPag.tPag, detPag.vPag);
            }

            y += 2;

            decimal troco = 0;
            decimal totalPago = 0;
            _nfe.infNFe.pag.ForEach(pag =>
            {
                troco += pag.vTroco ?? 0;
                totalPago += pag.detPag.Sum(x => x.vPag);
            });

            if (troco > 0)
            {
                AdicionarTexto textoTroco = new AdicionarTexto(g, "Troco R$ (TOTAL PAGO " + totalPago.ToString("C") + ")", 7);
                textoTroco.Desenhar(x, y);

                AdicionarTexto textoTrocoValor = new AdicionarTexto(g, troco.ToString("C"), 7);
                int textoTrocoValorX = (larguraLinhaMargemDireita - textoTrocoValor.Medida.Largura);
                textoTrocoValor.Desenhar(textoTrocoValorX, y);
                y += textoTroco.Medida.Altura;
            }
        }

        private void DesenharInfoConsultaSefaz(Graphics g)
        {
            AdicionarTexto textoConsulteChave = new AdicionarTexto(g, "Consulte pela Chave de Acesso em", 7);
            AdicionarTextoCentralizado(textoConsulteChave);

            AdicionarTexto textoUrlSefaz = new AdicionarTexto(g, _configDanfe.NFCeUrlConsultaSefaz, 7);
            AdicionarTextoCentralizado(textoUrlSefaz);

            string novaChave = FormatarChaveAcesso(_nfe);

            AdicionarTexto chave = new AdicionarTexto(g, novaChave, 7);
            AdicionarTextoCentralizado(chave);

            y += 10;
        }

        private void DesenharMensagemConsumidor(Graphics g)
        {
            string mensagemConsumidor = MontarMensagemConsumidor(_nfe.infNFe.dest);

            AdicionarTexto consumidor = new AdicionarTexto(g, mensagemConsumidor, 9);
            QuebraDeLinha quebraLinhaConsumidor = new QuebraDeLinha(consumidor,
                new ComprimentoMaximo(larguraLinhaMargemDireita), consumidor.Medida.Largura);
            consumidor = quebraLinhaConsumidor.DesenharComQuebras(g);
            AdicionarTextoCentralizado(consumidor);
        }

        private void DesenharMensagemDadosNFCe(Graphics g)
        {
            string mensagemDadosNfCe = MontarMensagemDadosNfce();
            AdicionarTexto dadosNfce = new AdicionarTexto(g, mensagemDadosNfCe, 7);
            AdicionarTextoCentralizado(dadosNfce);
        }

        private void DesenharDadosAutorizacao(Graphics g)
        {
            if (_nfe.infNFe.ide.tpEmis == DFeBR.EmissorNFe.Utilidade.Tipos.TipoEmissao.Normal)
            {
                var protocolo = _proc.protNFe.infProt;
                var textoProtocoloAutorizacao = new StringBuilder("Protocolo de autorização: ");
                textoProtocoloAutorizacao.Append(protocolo.nProt);
                AdicionarTexto protocoloAutorizacao = new AdicionarTexto(g, textoProtocoloAutorizacao.ToString(), 7);
                AdicionarTextoCentralizado(protocoloAutorizacao);

                StringBuilder textoDataAutorizacao = new StringBuilder("Data de autorização ");
                textoDataAutorizacao.Append(_proc.protNFe.infProt.dhRecbto.ToString("G"));
                AdicionarTexto dataAutorizacao = new AdicionarTexto(g, textoDataAutorizacao.ToString(), 7);
                AdicionarTextoCentralizado(dataAutorizacao);
            }
        }

        private void DesenharQrCode(Graphics g)
        {
            y += 8;

            string urlQrCode = ObtemUrlQrCode(_nfe, _cIdToken, _csc);

            using (var qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(urlQrCode, QRCodeGenerator.ECCLevel.M);
                QRCode qrCode = new QRCode(qrCodeData);
                var qrCodeImagemTmp = qrCode.GetGraphic(25);
                Bitmap qrCodeImagem = new Bitmap(qrCodeImagemTmp, new Size(100, 100));
                Image img = (Image)qrCodeImagem;
                int qrCodeImagemX = (larguraLinha - img.Size.Width) / 2;
                AdicionarImagem desenharQrCode = new AdicionarImagem(g, img, qrCodeImagemX, y);
                desenharQrCode.Desenhar();
                y += qrCodeImagem.Size.Height;
            }
        }

        private void DesenharTributosIncidentes(Graphics g)
        {
            decimal tributosIncidentes = _nfe.infNFe.total.ICMSTot.vTotTrib;

            if (tributosIncidentes != 0)
            {
                StringBuilder mensagemTributosTotais =
                    new StringBuilder("Tributos Totais Incidentes (Lei Federal 12.741/2012): R$");
                mensagemTributosTotais.Append(tributosIncidentes.ToString("N2"));

                AdicionarTexto tributosTotais = new AdicionarTexto(g, mensagemTributosTotais.ToString(), 7);
                AdicionarTextoCentralizado(tributosTotais);

                y += 5;

                LinhaHorizontal(g, x, y, larguraLinha);
            }
        }

        private void DesenharObservacoes(Graphics g)
        {
            string observacoes = string.Empty;

            if (_nfe?.infNFe?.infAdic != null)
                observacoes = _nfe.infNFe.infAdic.infCpl;

            if (!string.IsNullOrEmpty(observacoes))
            {
                y += 5;
                AdicionarTexto observacao = new AdicionarTexto(g, observacoes, 7);
                QuebraDeLinha quebraObservacao = new QuebraDeLinha(observacao,
                    new ComprimentoMaximo(larguraLinhaMargemDireita), observacao.Medida.Largura);
                observacao = quebraObservacao.DesenharComQuebras(g);
                observacao.Desenhar(x, y);

                y += observacao.Medida.Altura;
            }
        }

        private void DesenharCreditos(Graphics g)
        {
            y += 2;
            var creditos = "Desenvolvido por www.laranjeiras.dev";

            AdicionarTexto textoConsulteChave = new AdicionarTexto(g, creditos, 7);
            AdicionarTextoCentralizado(textoConsulteChave);
        }

        private void DesenharMensagemContingencia(Graphics g)
        {
            if (_nfe.infNFe.ide.tpEmis != DFeBR.EmissorNFe.Utilidade.Tipos.TipoEmissao.Normal)
            {
                y += 2;
                AdicionarTexto contingenciaTitulo = new AdicionarTexto(g, "EMITIDA EM CONTINGÊNCIA", 10);
                AdicionarTextoCentralizado(contingenciaTitulo);

                AdicionarTexto pendenteAutorizacaoTitulo = new AdicionarTexto(g, "Pendente de Autorização", 8);
                AdicionarTextoCentralizado(pendenteAutorizacaoTitulo);
                y += 2;
                LinhaHorizontal(g, x, y, larguraLinha);
            }
        }

        private void DesenharFormaPagamento(int x, int larguraLinhaMargemDireita, Graphics g, FormaPagamento? formaPagamento, decimal? vPag)
        {
            var _formaPagamento = formaPagamento.GetDescription();
            AdicionarTexto textoFormaPagamento = new AdicionarTexto(g, _formaPagamento, 7);
            textoFormaPagamento.Desenhar(x, y);

            AdicionarTexto textoValorFormaPagamento = new AdicionarTexto(g, vPag.Value.ToString("C"), 7);
            int textoValorFormaPagamentoX = (larguraLinhaMargemDireita - textoValorFormaPagamento.Medida.Largura);
            textoValorFormaPagamento.Desenhar(textoValorFormaPagamentoX, y);

            y += textoFormaPagamento.Medida.Altura;
        }

        private string MontarMensagemEnderecoEmitente()
        {
            var enderEmit = _nfe.infNFe.emit.enderEmit;

            string foneEmit = string.Empty;

            if (enderEmit.fone != null)
            {
                foneEmit = $"\nFONE: {enderEmit.fone}";
            }


            StringBuilder enderecoEmitenteBuilder = new StringBuilder();
            enderecoEmitenteBuilder.Append(enderEmit.xLgr);
            enderecoEmitenteBuilder.Append(" ");

            if (string.IsNullOrEmpty(enderEmit.nro))
            {
                enderecoEmitenteBuilder.Append("S/N, ");
            }

            if (!string.IsNullOrEmpty(enderEmit.nro))
            {
                enderecoEmitenteBuilder.Append(enderEmit.nro);
                enderecoEmitenteBuilder.Append(", ");
            }
            enderecoEmitenteBuilder.Append("\n");
            enderecoEmitenteBuilder.Append(enderEmit.xBairro);
            enderecoEmitenteBuilder.Append(", ");
            enderecoEmitenteBuilder.Append(enderEmit.xMun);
            enderecoEmitenteBuilder.Append(", ");
            enderecoEmitenteBuilder.Append(enderEmit.UF);
            enderecoEmitenteBuilder.Append(foneEmit);

            return enderecoEmitenteBuilder.ToString();
        }

        private string MontarMensagemRazaoSocial()
        {
            var emitente = _nfe.infNFe.emit;
            return string.IsNullOrEmpty(emitente.xFant) ? emitente.xNome : emitente.xFant;
        }

        private string MontarMensagemCpfCnpjIE()
        {
            var emitente = _nfe.infNFe.emit;
            return string.IsNullOrEmpty(emitente.CNPJ) ? $"CPF: {emitente.CPF}" : $"CNPJ: {emitente.CNPJ}    IE: {emitente.IE}";
        }

        private string MontarMensagemDadosNfce()
        {
            StringBuilder mensagem = new StringBuilder("NFC-e nº ");
            mensagem.Append(_nfe.infNFe.ide.nNF.ToString("D9"));
            mensagem.Append(" Série ");
            mensagem.Append(_nfe.infNFe.ide.serie.ToString("D3"));
            mensagem.Append(" ");
            mensagem.Append(_nfe.infNFe.ide.dhEmi.ToString("G"));
            mensagem.Append(" - ");
            mensagem.Append("Via consumidor");

            return mensagem.ToString();
        }

        private string MontarMensagemConsumidor(dest dest)
        {
            StringBuilder mensagem = new StringBuilder("CONSUMIDOR ");

            if (dest == null || (string.IsNullOrEmpty(dest.CPF) && string.IsNullOrEmpty(dest.CNPJ)))
            {
                mensagem.Append("NÃO IDENTIFICADO");
                return mensagem.ToString();
            }

            if (!string.IsNullOrEmpty(dest.idEstrangeiro))
            {
                mensagem.Append("Id ");
                mensagem.Append(dest.idEstrangeiro);
            }

            if (!string.IsNullOrEmpty(dest.CPF))
            {
                mensagem.Append("CPF ");
                mensagem.Append(dest.CPF);
            }

            if (!string.IsNullOrEmpty(dest.CNPJ))
            {
                mensagem.Append("CNPJ ");
                mensagem.Append(dest.CNPJ);
            }

            if (!string.IsNullOrEmpty(dest.xNome))
            {
                mensagem.Append(" ");
                mensagem.Append(dest.xNome);
            }

            enderDest enderecoDest = dest.enderDest;

            if (enderecoDest == null) return mensagem.ToString().Replace(", ,", ", ");

            string rua = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xLgr))
                rua = enderecoDest.xLgr;

            string numero = "S/N";
            if (!string.IsNullOrEmpty(enderecoDest.nro))
                numero = enderecoDest.nro;

            string bairro = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xBairro))
                bairro = enderecoDest.xBairro;

            string cidade = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.xMun))
                bairro = enderecoDest.xMun;

            string siglaUf = string.Empty;
            if (!string.IsNullOrEmpty(enderecoDest.UF))
                bairro = enderecoDest.UF;

            if (string.IsNullOrEmpty(rua)) return mensagem.ToString();
            mensagem.Append(" - ");
            mensagem.Append(rua);
            mensagem.Append(", ");
            mensagem.Append(numero);
            mensagem.Append(", ");
            mensagem.Append(bairro);
            mensagem.Append(", ");
            mensagem.Append(cidade);
            mensagem.Append(" - ");
            mensagem.Append(siglaUf);

            return mensagem.ToString().Replace(", ,", ", ");
        }

        private void AdicionarTextoCentralizado(AdicionarTexto texto)
        {
            int textoConsulteChaveX = ((larguraLinha - texto.Medida.Largura) / 2);
            texto.Desenhar(textoConsulteChaveX, y);
            y += texto.Medida.Altura;
        }

        private string ObtemUrlQrCode(NFe nfce, string idToken, string csc)
        {
            var url = _configDanfe.NFCeUrlConsultaQrCodeSefaz;

            const string pipe = "|";

            //Chave de Acesso da NFC-e 
            var chave = _nfe.infNFe.Id.Substring(3);

            //Identificação do Ambiente (1 – Produção, 2 – Homologação) 
            var ambiente = (int)_nfe.infNFe.ide.tpAmb;

            //Identificador do CSC (Código de Segurança do Contribuinte no Banco de Dados da SEFAZ). Informar sem os zeros não significativos
            var idCsc = Convert.ToInt16(csc);

            string dadosBase;

            if (_nfe.infNFe.ide.tpEmis == DFeBR.EmissorNFe.Utilidade.Tipos.TipoEmissao.ContingenciaOffLineNfce)
            {
                var diaEmi = _nfe.infNFe.ide.dhEmi.Day.ToString("D2");
                var valorNfce = _nfe.infNFe.total.ICMSTot.vNF.ToString("0.00").Replace(',', '.');
                var digVal = Nativo.Conversor.ObterHexDeString(_nfe.Signature.SignedInfo.Reference.DigestValue);
                dadosBase = string.Concat(chave, pipe, 2, pipe, ambiente, pipe, diaEmi, pipe, valorNfce, pipe, digVal, pipe, idCsc);
            }
            else
            {
                dadosBase = string.Concat(chave, pipe, 2, pipe, ambiente, pipe, idCsc);
            }

            var dadosSha1 = string.Concat(dadosBase, csc);
            var sh1 = Nativo.Conversor.ObterHexSha1DeString(dadosSha1);

            return string.Concat(url, dadosBase, pipe, sh1);
        }

        private string FormatarChaveAcesso(NFe nfce)
        {
            string chaveAcesso = nfce.infNFe.Id.Substring(3);
            string novaChave = string.Empty;
            int contaChaveAcesso = 0;

            foreach (char c in chaveAcesso)
            {
                contaChaveAcesso++;
                novaChave += c;

                if (contaChaveAcesso == 4)
                {
                    novaChave += " ";
                    contaChaveAcesso = 0;
                }
            }
            return novaChave;
        }

        private static AdicionarTexto CriaHeaderColuna(string texto, Graphics graphics, int x, int y)
        {
            AdicionarTexto coluna = new AdicionarTexto(graphics, texto, 7);
            coluna.Desenhar(x, y);
            return coluna;
        }

        private static void LinhaHorizontal(Graphics g, int x, int y, int larguraLinha)
        {
            new LinhaHorizontal(g, Pens.Black, x, y, larguraLinha, y).Desenhar();
        }

        private static int MontarLinhaTitulo(Graphics g, string texto, int tamanhoFonteTitulo, int larguraLogo, int x,
            int y, int larguraLinha)
        {
            AdicionarTexto adicionarTexto = new AdicionarTexto(g, texto, tamanhoFonteTitulo);
            ComprimentoMaximo larguraMaximaTexto = new ComprimentoMaximo((larguraLinha - larguraLogo));
            int laguraDoTexto = adicionarTexto.Medida.Largura;
            QuebraDeLinha quebrarLinha = new QuebraDeLinha(adicionarTexto, larguraMaximaTexto, laguraDoTexto);
            adicionarTexto = quebrarLinha.DesenharComQuebras(g);
            int posisaoXTexto = x + larguraLogo + (((larguraLinha - larguraLogo) - adicionarTexto.Medida.Largura) / 2);
            adicionarTexto.Desenhar(posisaoXTexto, y);
            y += adicionarTexto.Medida.Altura;
            return y;
        }
    }
}
