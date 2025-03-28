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
using file.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Test.Controllers
{
    public class FileControllerTest
    {
        private Mock<IImportService> _mockService;

        public FileControllerTest()
        {
            _mockService = new Mock<IImportService>();
        }

        [Fact]
        public async Task ImportFile()
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
            var controller = new FileController(_mockService.Object);

            //Act
            var result = await controller.ImportFile(file);

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task ImportFile_NoFiles()
        {
            //Arrange
            var file = new FileModel();
            var controller = new FileController(_mockService.Object);

            //Act
            var result = await controller.ImportFile(file);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<List<ValidationError>>(okResult.Value);

            //Assert
            Assert.Equal("NOFILE", response[0].Code);
            Assert.Equal("No file was recieved on the server", response[0].Description);
        }

    }
}
