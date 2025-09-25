using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.WebApi
{
    [TestFixture]
    public class ImagesControllerTests
    {
        private Mock<IFileStorageService> _fileStorageMock;
        private Mock<IPropertyImageService> _imageServiceMock;
        private ImagesController _controller;

        [SetUp]
        public void Setup()
        {
            _fileStorageMock = new Mock<IFileStorageService>();
            _imageServiceMock = new Mock<IPropertyImageService>();
            _controller = new ImagesController(_fileStorageMock.Object, _imageServiceMock.Object);
        }

        [Test]
        public async Task Upload_ShouldReturnCreated_WhenImageIsSaved()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.png";
            var contentType = "image/png";
            var fileLength = 100;

            _fileStorageMock
                .Setup(x => x.SavePropertyImageAsync(propertyId, fileMock.Object))
                .ReturnsAsync((fileName, contentType, fileLength));

            var newImageId = Guid.NewGuid();
            _imageServiceMock
                .Setup(x => x.AddAsync(propertyId, fileName, contentType, fileLength))
                .ReturnsAsync(newImageId);

            // Act
            var result = await _controller.Upload(propertyId, fileMock.Object);

            // Assert
            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            Assert.AreEqual(201, created.StatusCode);
        }

        [Test]
        public async Task List_ShouldReturnOkWithImages()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var images = new[] { new PropertyImage { IdPropertyImage = Guid.NewGuid(), FileName = "file.png" } };

            _imageServiceMock
                .Setup(x => x.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(images);

            // Act
            var result = await _controller.List(propertyId);

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(images, ok.Value);
        }

        [Test]
        public async Task Download_ShouldReturnFile_WhenImageExists()
        {
            // Arrange
            var imageId = Guid.NewGuid();
            var image = new PropertyImage { IdPropertyImage = imageId, FileName = "file.png", ContentType = "image/png" };

            _imageServiceMock
                .Setup(x => x.GetByIdAsync(imageId))
                .ReturnsAsync(image);

            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3 });
            _fileStorageMock
                .Setup(x => x.GetPropertyImageStreamAsync(image.FileName))
                .ReturnsAsync(memoryStream);

            // Act
            var result = await _controller.Download(imageId);

            // Assert
            var fileResult = result as FileStreamResult;
            Assert.IsNotNull(fileResult);
            Assert.AreEqual("image/png", fileResult.ContentType);
            Assert.AreEqual("file.png", fileResult.FileDownloadName);
        }

        [Test]
        public async Task Download_ShouldReturnNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var imageId = Guid.NewGuid();
            _imageServiceMock
                .Setup(x => x.GetByIdAsync(imageId))
                .ReturnsAsync((PropertyImage)null);

            // Act
            var result = await _controller.Download(imageId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var imageId = Guid.NewGuid();
            _imageServiceMock
                .Setup(x => x.DeleteAsync(imageId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(imageId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
