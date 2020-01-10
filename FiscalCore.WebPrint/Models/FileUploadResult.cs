namespace FiscalCore.WebPrint.Models
{
    public class FileUploadResult
    {
        public long Length { get; set; }
        public string Name { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public byte[] Bytes { get; set; }
    }
}
