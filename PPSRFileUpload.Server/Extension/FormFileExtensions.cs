using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace File.Extension
{
    public static class FormFileExtensions
    {
        public static Stream ReadStream(this IFormFile file, List<string> contentTypes)
        {
            if (!contentTypes.Any(x => x == file.ContentType))
            {
                throw new ArgumentException($"Unexpected file format: '{file.ContentType}'.");
            }

            return file.OpenReadStream();
        }
    }
}
