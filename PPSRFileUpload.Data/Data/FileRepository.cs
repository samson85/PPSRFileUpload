using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Data.Model;

namespace Data.Model
{
    public class FileRepository : IFileRepository
    {
        protected readonly AppDbContext _db;
        public FileRepository(AppDbContext db)
        {
            _db = db;
        }
        public void UpsertFileConfig(bool create, List<FileConfig> fileEntries)
        {
            if (create)
            {
                _db.FileConfig.AddRange(fileEntries);
            }
            else
            {
                _db.FileConfig.UpdateRange(fileEntries);
            }
        }

        public void AddImportedFileName(ImportedFiles importedFiles)
        {
            _db.ImportedFiles.Add(importedFiles);

        }

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        public List<FileConfig> GetAllFileConfigAsync()
        {
            var results = _db.FileConfig.ToList();
            return results;
        }
        public List<ImportedFiles> GetAllImportedFilesAsync()
        {
            var results = _db.ImportedFiles.ToList();
            return results;
        }

    }
}
