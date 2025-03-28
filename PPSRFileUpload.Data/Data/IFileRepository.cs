using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Model
{
    public interface IFileRepository
    {
        void UpsertFileConfig(bool create, List<FileConfig> newEntries);

        void AddImportedFileName(ImportedFiles newEntries);
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        List<FileConfig> GetAllFileConfigAsync();
        public List<ImportedFiles> GetAllImportedFilesAsync();


    }
}
