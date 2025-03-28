using File.Extension;
using Model.FileModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Contract;
using Moq;
using Import.Service;
using file.Controllers;
using Data.Model;

namespace Test.Services
{
    public class ImportServiceTest
    {
        private Mock<IFileRepository> _fileRepo;
        private IImportService _service;

        public ImportServiceTest()
        {
            _fileRepo = new Mock<IFileRepository>();
            _service = new ImportService(_fileRepo.Object);
        }

        [Fact]
        public async Task ValidateAndImportAsync_NoContent()
        {
            // Arrange
            var file = new FileModel();
            var stream = new MemoryStream();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.ContentType)
                .Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            file.FormFile = mockFile.Object;
            file.FileName = "SPGR.csv";


            //Act
            var result = await _service.ValidateAndImportAsync(file);

            //Assert
            Assert.Equal("EMPTY", result[0].Code);
            Assert.Equal("File is Empty", result[0].Description);
        }

        [Fact]
        public async Task ValidateAndImportAsync_No_ValidSubmission()
        {
            // Arrange
            var file = new FileModel();
            var stream = new MemoryStream();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.ContentType)
                .Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(stream);
            file.FormFile = mockFile.Object;
            file.FileName = "SPGR.csv";

            //Act
            var result = await _service.ValidateAndImportAsync(file);

            //Assert
            Assert.Equal("NODATA", result[0].Code);
            Assert.Equal("File do not have proper data", result[0].Description);
        }
        [Fact]
        public async Task ValidateAndImportAsync_File_Already_Imported()
        {
            // Arrange

            _fileRepo.Setup(x => x.GetAllImportedFilesAsync())
                .Returns(GetDummyImportedFileNamesForMock()).Verifiable();

            var file = new FileModel();
            var stream = new MemoryStream();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.ContentType)
                .Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(stream);
            file.FormFile = mockFile.Object;
            file.FileName = "Testdata.csv";

            //Act
            var result = await _service.ValidateAndImportAsync(file);

            //Assert
            Assert.Equal("Testdata.csv is already Imported", result[0].Description);
        }
        [Fact]
        public async Task ValidateAndImportAsync_Import_Max_File_Size()
        {
            // Arrange
            var file = new FileModel();
            Stream stream = System.IO.File.OpenRead("Services/SampledataBigSize.csv");
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.ContentType)
                .Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(stream);
            file.FormFile = mockFile.Object;

            //Act
            var result = await _service.ValidateAndImportAsync(file);

            //Assert
            Assert.Equal("File size is big. The maximum file size for a CSV is 25MB", result[0].Description);
        }
        [Fact]
        public async Task ValidateAndImportAsync_Import_File_RecordsDetail()
        {
            // Arrange
            _fileRepo.Setup(x => x.GetAllFileConfigAsync())
                .Returns(GetDummyFileConfigsForMock()).Verifiable();

            _fileRepo.Setup(x => x.GetAllImportedFilesAsync())
                .Returns(GetDummyImportedFileNamesForMock()).Verifiable();

            var file = new FileModel();
            Stream stream = System.IO.File.OpenRead("Services/Sampledata.csv");
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(x => x.ContentType)
                .Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mockFile.Setup(x => x.OpenReadStream())
                .Returns(stream);
            file.FormFile = mockFile.Object;
            file.FileName = "Sampledata.csv";

            //Act
            var result = await _service.ValidateAndImportAsync(file);
            
            //Assert

            // Added records check
            Assert.Equal("Number of added records: 3", result.Where(x => x.Code == "ADD").First().Description);
            
            // Submitted records check
            Assert.Equal("Number of submitted records: 11", result.Where(x => x.Code == "SUBMIT").First().Description);

            // Invalid records check
            Assert.Equal("Number of invalid records: 1", result.Where(x => x.Code == "INVALID").First().Description);

            // Updated records check
            Assert.Equal("Number of updated records: 1", result.Where(x => x.Code == "UPDATE").First().Description);

            // Duplicate records check
            Assert.Equal("Number of duplicated records: 6", result.Where(x => x.Code == "DUPLICATE").First().Description);

            // Processed records check
            Assert.Equal("Number of Processed records: 4", result.Where(x => x.Code == "PROCESSED").First().Description);
        }
        private List<FileConfig> GetDummyFileConfigsForMock()
        {
            return new List<FileConfig>
            {
                new FileConfig
                {
                    FirstName = "Bryson",
                    MiddleName = "James",
                    LastName = "Bernal",
                    Vin = "2GCEC19Z8L1159877",
                    Startdate = DateOnly.FromDateTime( new DateTime(2025, 02, 23)),
                    Duration = DurationEnum.SevenYears,
                    SpgAcn = "001 000 004",
                    SpgOrganization = "Company A"

                },
                new FileConfig
                {
                    FirstName = "Murphy",
                    MiddleName = "Peter, Lee",
                    LastName = "O’Connor",
                    Vin = "ZAMGJ45A480037578",
                    Startdate = DateOnly.FromDateTime(new DateTime(2025, 02, 24)),
                    Duration = DurationEnum.TewntyFiveyYears,
                    SpgAcn = "002 249 998",
                    SpgOrganization = "Company B"
                },
                new FileConfig
                {
                    FirstName = "Emmy",
                    MiddleName = null,
                    LastName = "Phillips",
                    Vin = "JH4DB1671PS002584",
                    Startdate = DateOnly.FromDateTime(new DateTime(2016, 01, 24)),
                    Duration = DurationEnum.NotApplicaple,
                    SpgAcn = "006 749 980",
                    SpgOrganization = "Company B"
                }
            };

        }
        private List<ImportedFiles> GetDummyImportedFileNamesForMock()
        {
            return new List<ImportedFiles>
            {
                 new ImportedFiles
                 {
                   FileName= "Testdata.csv"
                 }
            };

        }

    }
}
