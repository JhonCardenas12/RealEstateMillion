using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.Application
{
    [TestFixture]
    public class PropertyImageServiceTests
    {
        private Mock<IPropertyImageRepository> _repoMock;
        private PropertyImageService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IPropertyImageRepository>();
            _service = new PropertyImageService(_repoMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldReturnId_AndCallRepoWithCorrectEntity()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var returnedId = Guid.NewGuid();
            PropertyImage captured = null;

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<PropertyImage>()))
                .Callback<PropertyImage>(pi => captured = pi)
                .ReturnsAsync(returnedId);

            // Act
            var result = await _service.AddAsync(propertyId, "house.jpg", "image/jpeg", 12345L);

            // Assert
            Assert.AreEqual(returnedId, result);
            Assert.IsNotNull(captured);
            Assert.AreEqual(propertyId, captured.IdProperty);
            Assert.AreEqual("house.jpg", captured.FileName);
            Assert.AreEqual("image/jpeg", captured.ContentType);
            Assert.AreEqual(12345L, captured.Size);
            Assert.That(captured.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<PropertyImage>()), Times.Once);
        }

        [Test]
        public async Task GetByPropertyIdAsync_ShouldReturnImages()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var images = new List<PropertyImage>
            {
                new PropertyImage { IdPropertyImage = Guid.NewGuid(), IdProperty = propertyId, FileName = "a.png" },
                new PropertyImage { IdPropertyImage = Guid.NewGuid(), IdProperty = propertyId, FileName = "b.png" }
            };

            _repoMock.Setup(r => r.GetByPropertyIdAsync(propertyId)).ReturnsAsync(images);

            // Act
            var result = await _service.GetByPropertyIdAsync(propertyId);

            // Assert
            Assert.IsNotNull(result);
            var list = result.ToList();
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Any(x => x.FileName == "a.png"));
            Assert.IsTrue(list.Any(x => x.FileName == "b.png"));
            _repoMock.Verify(r => r.GetByPropertyIdAsync(propertyId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnImage()
        {
            // Arrange
            var id = Guid.NewGuid();
            var image = new PropertyImage { IdPropertyImage = id, FileName = "pic.jpg" };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(image);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.IdPropertyImage);
            Assert.AreEqual("pic.jpg", result.FileName);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallRepo()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task ToggleEnableAsync_ShouldCallRepo()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.ToggleEnableAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _service.ToggleEnableAsync(id);

            // Assert
            _repoMock.Verify(r => r.ToggleEnableAsync(id), Times.Once);
        }

        [Test]
        public void AddAsync_ShouldThrow_WhenRepoThrows()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            _repoMock.Setup(r => r.AddAsync(It.IsAny<PropertyImage>()))
                     .ThrowsAsync(new InvalidOperationException("DB error"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddAsync(propertyId, "x.png", "image/png", 10L));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<PropertyImage>()), Times.Once);
        }
    }
}
