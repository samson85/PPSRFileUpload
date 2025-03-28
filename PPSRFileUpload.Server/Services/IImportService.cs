using Model.FileModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contract
{
    public interface IImportService
    {
        Task<List<ValidationError>> ValidateAndImportAsync(FileModel formFile);

    }
}
