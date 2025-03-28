using File.Extension;
using Model.FileModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Contract;

namespace file.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IImportService _service;

        public FileController(IImportService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> ImportFile([FromForm] FileModel formFile)
        {
            var entriesInfo = new List<ValidationError>();
            if (formFile == null || formFile.FormFile == null)
            {
                entriesInfo.Add(new ValidationError { Code = "NOFILE", Description = "No file was recieved on the server" });
            }
            else
            {
                entriesInfo = await _service.ValidateAndImportAsync(formFile);
            }
            return Ok(entriesInfo);
        }

    }
}
