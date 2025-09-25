using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.Application
{
    [TestFixture]
    public class OwnerServiceTests
    {
        private Mock<IOwnerRepository> _ownerRepoMock;
        private Mock<IUnitOfWork> _uowMock;
        private OwnerService _ownerService;

        [SetUp]
        public void Setup()
        {
            _ownerRepoMock = new Mock<IOwnerRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _ownerService = new OwnerService(_ownerRepoMock.Object, _uowMock.Object);
        }

        [Test]
        public async Task CreateAsync_ShouldCommit_WhenSuccess()
        {
            // Arrange
            var dto = new OwnerCreateDto
            {
                Name = "Test Owner",
                Address = "123 Street",
                ContactEmail = "test@test.com",
                Phone = "555-123",
                Birthday = new DateTime(1990, 1, 1),
                PhotoFileName = "photo.png"
            };

            var newId = Guid.NewGuid();
            _ownerRepoMock.Setup(r => r.AddAsync(It.IsAny<Owner>())).ReturnsAsync(newId);

            // Act
            var result = await _ownerService.CreateAsync(dto);

            // Assert
            Assert.AreEqual(newId, result);
            _uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            _uowMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Test]
        public void CreateAsync_ShouldRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new OwnerCreateDto { Name = "Fail Owner" };
            _ownerRepoMock.Setup(r => r.AddAsync(It.IsAny<Owner>())).ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _ownerService.CreateAsync(dto));
            _uowMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnOwner()
        {
            // Arrange
            var id = Guid.NewGuid();
            var owner = new Owner { IdOwner = id, Name = "Owner1" };
            _ownerRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(owner);

            // Act
            var result = await _ownerService.GetByIdAsync(id);

            // Assert
            Assert.AreEqual("Owner1", result.Name);
        }

        [Test]
        public async Task ListAsync_ShouldReturnOwners()
        {
            // Arrange
            var owners = new List<Owner> { new Owner { IdOwner = Guid.NewGuid(), Name = "Owner1" } };
            _ownerRepoMock.Setup(r => r.GetListAsync(null)).ReturnsAsync(owners);

            // Act
            var result = await _ownerService.ListAsync(null);

            // Assert
            Assert.AreEqual(1, ((List<Owner>)result).Count);
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenOwnerNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _ownerRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Owner)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _ownerService.UpdateAsync(id, new OwnerUpdateDto { Name = "Updated" }));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateOwner_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Owner { IdOwner = id, Name = "Old" };
            _ownerRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            var dto = new OwnerUpdateDto { Name = "New", Address = "Addr", ContactEmail = "mail", Phone = "111" };

            // Act
            await _ownerService.UpdateAsync(id, dto);

            // Assert
            _ownerRepoMock.Verify(r => r.UpdateAsync(It.Is<Owner>(o => o.Name == "New")), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallRepo()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _ownerService.DeleteAsync(id);

            // Assert
            _ownerRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task SetPhotoAsync_ShouldCallRepo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fileName = "pic.png";

            // Act
            await _ownerService.SetPhotoAsync(id, fileName, "image/png");

            // Assert
            _ownerRepoMock.Verify(r => r.SetPhotoAsync(id, fileName, "image/png"), Times.Once);
        }
    }
}
