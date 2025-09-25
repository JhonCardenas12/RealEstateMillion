using NUnit.Framework;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RealEstate.Infrastructure;
using System.Collections.Generic;

namespace RealEstate.Tests.Infrastructure
{
    [TestFixture]
    public class LocalFileStorageServiceTests
    {
        private string _tempFolder;
        private LocalFileStorageService _service;

        [SetUp]
        public void Setup()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "RealEstateTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempFolder);

            var settings = new Dictionary<string, string>
            {
                { "FileStorage:ImagesFolder", _tempFolder }
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _service = new LocalFileStorageService(config);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempFolder))
            {
                Directory.Delete(_tempFolder, recursive: true);
            }
        }

        private IFormFile CreateFakeFormFile(string fileName, string contentType, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Test]
        public async Task SavePropertyImageAsync_ShouldSaveFileAndReturnMetadata()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var file = CreateFakeFormFile("test.jpg", "image/jpeg", "dummy data");

            // Act
            var result = await _service.SavePropertyImageAsync(propertyId, file);

            // Assert
            Assert.IsNotNull(result.FileName);
            Assert.IsTrue(result.FileName.StartsWith(propertyId.ToString("D")));
            Assert.AreEqual("image/jpeg", result.ContentType);
            Assert.AreEqual(file.Length, result.Size);

            var fullPath = Path.Combine(_tempFolder, result.FileName);
            Assert.IsTrue(File.Exists(fullPath));
            var savedContent = await File.ReadAllTextAsync(fullPath);
            Assert.AreEqual("dummy data", savedContent);
        }

        [Test]
        public void SavePropertyImageAsync_ShouldThrow_WhenFileIsEmpty()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var emptyFile = CreateFakeFormFile("empty.png", "image/png", "");

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.SavePropertyImageAsync(propertyId, emptyFile));

            Assert.That(ex.Message, Does.Contain("File is empty"));
        }

        [Test]
        public async Task GetPropertyImageStreamAsync_ShouldReturnStream_WhenFileExists()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var file = CreateFakeFormFile("pic.png", "image/png", "hello image");
            var meta = await _service.SavePropertyImageAsync(propertyId, file);

            // Act
            var stream = await _service.GetPropertyImageStreamAsync(meta.FileName);

            // Assert
            Assert.IsNotNull(stream);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            Assert.AreEqual("hello image", content);
        }

        [Test]
        public async Task GetPropertyImageStreamAsync_ShouldReturnNull_WhenFileDoesNotExist()
        {
            // Act
            var stream = await _service.GetPropertyImageStreamAsync("nonexistent.png");

            // Assert
            Assert.IsNull(stream);
        }
    }
}
