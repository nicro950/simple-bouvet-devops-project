using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bouvet.Syndicate.TestProject.Helpers.BlobStorage;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Bouvet.Syndicate.TestProject.UnitTests
{
    public class FileUploadValidation
    {
        private const long Kilo = 1 << 10;
        private const long Mega = Kilo << 10;

        // Supported:       
        //      .pdf  - Pdf
        //      .docx - Microsoft Word Open format, 
        //      .xlsx - Microsoft Excel open format, 
        //      .odt  - Open Office Text
        //      .ods  - Open Office Spreadsheet
        // Maybe supported: 
        //      .doc  - Microsoft Word Old format
        //      .xls  - Microsoft Excel Old format
        [Theory]
        [InlineData("image/png", false)]
        [InlineData("image/jpeg", false)]
        [InlineData("Something", false)]
        [InlineData("application/pdf", true)] // pdf
        [InlineData("application/msword", true)] // doc
        [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", true)] //docx
        [InlineData("application/vnd.ms-excel", true)] // xls
        [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", true)] //xlsx
        [InlineData("application/vnd.oasis.opendocument.spreadsheet", true)] // ods
        [InlineData("application/vnd.oasis.opendocument.text", true)] // odt
        public void CheckMimeTypeValidation(string mimeType, bool expected)
        {
            var result = MimeValidator.IsValidContentType(mimeType);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(".png", false)]
        [InlineData(".jpeg", false)]
        [InlineData(".txt", false)]
        [InlineData(".pdf", true)]
        [InlineData(".doc", true)]
        [InlineData(".docx", true)]
        [InlineData(".xls", true)]
        [InlineData(".xlsx", true)]
        [InlineData(".ods", true)]
        [InlineData(".odt", true)]
        public void CheckFileExtension(string extension, bool expected)
        {
            var result = MimeValidator.IsValidFileExtension(extension);
            Assert.Equal(expected, result);
        }
    }

    internal class FormFile : IFormFile
    {
        public string ContentDisposition { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public IHeaderDictionary Headers { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public void CopyTo(Stream target)
        {
            throw new System.NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            throw new System.NotImplementedException();
        }
    }
}
