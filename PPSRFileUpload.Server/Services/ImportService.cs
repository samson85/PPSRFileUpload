using Core.Contract;
using CsvHelper;
using CsvHelper.Configuration;
using Data.Model;
using File.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model.FileModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Import.Service
{
    public class CustomComparer : EqualityComparer<FileConfig>
    {
        public override int GetHashCode(FileConfig a)
        {
            int hCode = a.GetHashCode() ^ a.GetHashCode();
            return hCode.GetHashCode();
        }

        public override bool Equals(FileConfig a1, FileConfig a2)
        {
            return a1.FirstName.Equals(a2.FirstName) && a1.MiddleName.Equals(a2.MiddleName) &&
                a1.LastName.Equals(a2.LastName) && a1.Vin.Equals(a2.Vin) &&
                a1.Startdate.Equals(a2.Startdate) && a1.Duration.Equals(a2.Duration) &&
                a1.SpgAcn.Equals(a2.SpgAcn) && a1.SpgOrganization.Equals(a2.SpgOrganization);
        }
    }
    public class ImportService : IImportService
    {
        private readonly IFileRepository _fileRepo;


        public ImportService(IFileRepository fileRepo)
        {
            _fileRepo = fileRepo;
        }

        public async Task<List<ValidationError>> ValidateAndImportAsync(FileModel formFile)
        {
            var entriesInfo = new List<ValidationError>();

            var contentTypes = new List<string>
            {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "application/vnd.ms-excel",
                 "application/octet-stream",
                 "application/csv",
                "text/csv"
            };
            using var stream = formFile.FormFile.ReadStream(contentTypes);
            if (stream == null)
            {
                entriesInfo.Add(new ValidationError { Code = "EMPTY", Description = "File is Empty" });
                return entriesInfo;
            }

            //Checking File Size
            if (stream.Length > 25 * 1024 * 1024)
            {
                entriesInfo.Add(new ValidationError { Code = "BIGSIZE", Description = "File size is big. The maximum file size for a CSV is 25MB" });
                return entriesInfo;
            }

            var alreadyImportedFiles = _fileRepo.GetAllImportedFilesAsync();

            //Checking if file is already imported
            if (alreadyImportedFiles != null)
            {
                if (alreadyImportedFiles.Where(x => x.FileName == formFile.FileName).Any())
                {
                    entriesInfo.Add(new ValidationError { Code = "EXIST", Description = $"{formFile.FileName} is already Imported" });
                    return entriesInfo;
                }
            }

            entriesInfo = await ConvertToEntitieAndSaveAsync(stream, entriesInfo, formFile.FileName);
            return entriesInfo;
        }
        private async Task<List<ValidationError>> ConvertToEntitieAndSaveAsync(Stream stream, List<ValidationError> entriesInfo, string fileName)
        {
            try
            {
                List<FileConfig> fileConfigs = new List<FileConfig>();
                StreamReader sr = new StreamReader(stream);

                bool hasHeaders = string.IsNullOrEmpty(sr.ReadLine());
                bool hasRow = false;
                var invalidRowsCount = 0;
                var submittedRowsCount = 0;
                var duplicateRowsCount = 0;
                FileConfig fileConfig;
                var existingFileConfigs = _fileRepo.GetAllFileConfigAsync();

                while (!sr.EndOfStream)
                {
                    submittedRowsCount++;

                    string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)", default, TimeSpan.FromMinutes(1));

                    if (rows != null)
                    {
                        if (string.IsNullOrEmpty(rows[0]) || string.IsNullOrEmpty(rows[2]) || string.IsNullOrEmpty(rows[3]) || string.IsNullOrEmpty(rows[6]) || string.IsNullOrEmpty(rows[4]) || string.IsNullOrEmpty(rows[7]))
                        {
                            invalidRowsCount++;
                        }
                        else
                        {
                            fileConfig = new FileConfig();
                            hasRow = true;

                            fileConfig.FirstName = rows[0];
                            fileConfig.MiddleName = rows[1];
                            fileConfig.LastName = rows[2];
                            fileConfig.Vin = rows[3];
                            fileConfig.Startdate = DateOnly.FromDateTime(Convert.ToDateTime(rows[4]));
                            fileConfig.Duration = string.IsNullOrEmpty(rows[5]) ? DurationEnum.NotApplicaple : rows[5] == "7" ? DurationEnum.SevenYears : rows[5] == "25" ? DurationEnum.TewntyFiveyYears : DurationEnum.NotApplicaple;
                            fileConfig.SpgAcn = rows[6];
                            fileConfig.SpgOrganization = rows[7];
                            fileConfigs.Add(fileConfig);
                        }
                    }
                }
                if (!hasRow && hasHeaders)
                {
                    entriesInfo.Add(new ValidationError { Code = "NODATA", Description = "File do not have proper data" });
                    return entriesInfo;
                }
                entriesInfo.Add(new ValidationError { Code = "SUBMIT", Description = $"Number of submitted records: {submittedRowsCount.ToString()}" });
                entriesInfo.Add(new ValidationError { Code = "INVALID", Description = $"Number of invalid records: {invalidRowsCount}" });

                // find update records from CSV file records checking with existing DB records
                var updateFileConfigs = existingFileConfigs.Where(e => fileConfigs.Any(f => f.FirstName == e.FirstName && f.LastName == e.LastName && f.MiddleName == e.MiddleName && f.Vin == e.Vin && f.SpgAcn == e.SpgAcn)).ToList();


                entriesInfo.Add(new ValidationError { Code = "UPDATE", Description = $"Number of updated records: {updateFileConfigs.Count()}" });


                // remove update records from CSV file records
                var newFileConfigs = fileConfigs.Except(updateFileConfigs, new CustomComparer()).ToList();

                var newRecordsCount = newFileConfigs.Count();

                // remove duplicate VIN records from CSV file records with checking with existing DB records -->	Each vehicle can only have one owner
                newFileConfigs = newFileConfigs.Where(e => !existingFileConfigs.Any(f => f.Vin == e.Vin)).ToList();

                // remove duplicate SPG records from CSV file records with checking with existing DB records
                newFileConfigs = newFileConfigs.Where(e => !existingFileConfigs.Any(f => f.SpgAcn == e.SpgAcn)).ToList();

                // Find duplicate VIN records from CSV file records 
                var duplicateRecordsFromFile = newFileConfigs.GroupBy(x => new { x.Vin }).Where(g => g.Count() > 1).Select(y => y.FirstOrDefault()).ToList();

                // remove duplicate records from new records
                var addFileConfigs = newFileConfigs.Except(duplicateRecordsFromFile, new CustomComparer()).ToList();

                // Find duplicate SPG records from CSV file records
                duplicateRecordsFromFile = addFileConfigs.GroupBy(x => new { x.SpgAcn }).Where(g => g.Count() > 1).Select(y => y.FirstOrDefault()).ToList();

                // remove SPG duplicate records from new records
                addFileConfigs = addFileConfigs.Except(duplicateRecordsFromFile, new CustomComparer()).ToList();

                duplicateRowsCount = newRecordsCount - addFileConfigs.Count();

                entriesInfo.Add(new ValidationError { Code = "DUPLICATE", Description = $"Number of duplicated records: {duplicateRowsCount}" });


                entriesInfo.Add(new ValidationError { Code = "ADD", Description = $"Number of added records: {addFileConfigs.Count()}" });

                entriesInfo.Add(new ValidationError { Code = "PROCESSED", Description = $"Number of Processed records: {addFileConfigs.Count() + updateFileConfigs.Count()}" });

                if (updateFileConfigs.Any())
                {
                    //update If a record with the same grantor, VIN, and SPG already exists
                    _fileRepo.UpsertFileConfig(false, updateFileConfigs);
                }
                if (addFileConfigs.Any())
                {
                    _fileRepo.UpsertFileConfig(true, addFileConfigs); // add new records
                }
                var importedFiles = new ImportedFiles
                {
                    FileName = fileName
                };

                _fileRepo.AddImportedFileName(importedFiles); // add Imported filenames into DB
                await _fileRepo.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                entriesInfo.Add(new ValidationError { Code = "ERROR", Description = $"Error Occured: {ex}" });
            }
            return entriesInfo;
        }
    }
}
