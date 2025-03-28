using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Model.FileModel
{
    public class FileModel
    {
        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
