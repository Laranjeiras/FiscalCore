using FiscalCore.WebPrint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using ZionDanfe;

namespace FiscalCore.WebPrint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        [HttpPost]
        [Route("GerarDanfe")]
        public async Task<IActionResult> GerarDanfe(IFormFile file)
        {
            try
            {
                if (file == null)
                    return NoContent();

                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        await file.CopyToAsync(outputStream);
                        var bytes = outputStream.ToArray();
                        var xmlText = Encoding.UTF8.GetString(bytes);

                        var modelo = ZionDanfe.Modelo.DanfeViewModelCreator.CriarDeStringXml(xmlText);
                        var pdfStream = new MemoryStream();

                        using (var danfe = new Danfe(modelo))
                        {
                            danfe.ViewModel.DefinirTextoCreditos("Desenvolvido por www.laranjeiras.dev / (21)97161-4935 / (zideun@outlook.com)");
                            danfe.Gerar();
                            var bytesPdf = danfe.ObterPdfBytes(pdfStream);
                            return File(bytesPdf, "Application/pdf", $"{modelo.ChaveAcesso}.pdf");
                        }
                    }
                }
                catch
                {
                    var result = new FileUploadResult() { 
                        Name = file.FileName,
                        Length = file.Length,
                        Success = false,
                        Message = "Ocorreu um erro" 
                    };
                    return BadRequest(result);
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Endpoint/Método para receber Xmls zipada e ler o conteúdo
        /// Ideia: receber xmls autorizadas e salvar em banco de dados.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/api/Pdf/Unzip")]
        public async Task<IActionResult> UnzipFileFromApi(IList<IFormFile> files)
        {
            var listaStringXmls = new List<string>();
            foreach (var file in files)
            {
                var archive = new ZipArchive(file.OpenReadStream());
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var stm = entry.Open();
                    using (var outputStream = new MemoryStream())
                    {
                        await stm.CopyToAsync(outputStream);
                        var bytes = outputStream.ToArray();
                        var xmlText = Encoding.UTF8.GetString(bytes);
                        listaStringXmls.Add(xmlText);
                    }
                }            
            }

            return Ok(listaStringXmls);
        }
    }
}