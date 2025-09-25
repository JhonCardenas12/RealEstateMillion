using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using AutoMapper;

namespace RealEstate.Tests.Application
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _repoMock;
        private Mock<IUnitOfWork> _uowMock;
        private Mock<IMapper> _mapperMock;
        private PropertyService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IPropertyRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new PropertyService(_repoMock.Object, _uowMock.Object, _mapperMock.Object);
        }

        [Test]
        public void CreateAsync_ShouldThrow_WhenPriceIsZero()
        {
            var dto = new PropertyCreateDto { Price = 0 };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.CreateAsync(dto));
        }

        [Test]
        public async Task CreateAsync_ShouldCommit_WhenSuccess()
        {
            // Arrange
            var dto = new PropertyCreateDto { Name = "House", Price = 100000 };
            var mapped = new Property { Name = "House", Price = 100000 };
            var newId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<Property>(dto)).Returns(mapped);
            _repoMock.Setup(r => r.AddAsync(mapped)).ReturnsAsync(newId);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.AreEqual(newId, result);
            _uowMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            _uowMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Test]
        public void CreateAsync_ShouldRollback_WhenExceptionThrown()
        {
            var dto = new PropertyCreateDto { Name = "Fail", Price = 200000 };
            var mapped = new Property { Name = "Fail", Price = 200000 };

            _mapperMock.Setup(m => m.Map<Property>(dto)).Returns(mapped);
            _repoMock.Setup(r => r.AddAsync(mapped)).ThrowsAsync(new Exception("DB Error"));

            Assert.ThrowsAsync<Exception>(async () => await _service.CreateAsync(dto));
            _uowMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProperty()
        {
            var id = Guid.NewGuid();
            var property = new Property { IdProperty = id, Name = "Test House" };

            _repoMock.Setup(r => r.GetByIdDetailedAsync(id)).ReturnsAsync(property);

            var result = await _service.GetByIdAsync(id);

            Assert.AreEqual("Test House", result.Name);
        }

        [Test]
        public async Task ListAsync_ShouldReturnProperties()
        {
            var list = new List<Property> { new Property { IdProperty = Guid.NewGuid(), Name = "P1" } };
            _repoMock.Setup(r => r.GetListAsync(null)).ReturnsAsync(list);

            var result = await _service.ListAsync(null);

            Assert.AreEqual(1, ((List<Property>)result).Count);
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenPropertyNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Property)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _service.UpdateAsync(id, new PropertyUpdateDto { Name = "New Name" }));
        }

        [Test]
        public async Task UpdateAsync_ShouldMapAndUpdate_WhenExists()
        {
            var id = Guid.NewGuid();
            var existing = new Property { IdProperty = id, Name = "Old" };
            var dto = new PropertyUpdateDto { Name = "New" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await _service.UpdateAsync(id, dto);

            _mapperMock.Verify(m => m.Map(dto, existing), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Test]
        public async Task ChangePriceAsync_ShouldCallRepo()
        {
            var id = Guid.NewGuid();

            await _service.ChangePriceAsync(id, 500000, "Correction");

            _repoMock.Verify(r => r.ChangePriceAsync(id, 500000, "Correction"), Times.Once);
        }

        [Test]
        public async Task BulkUpsertAsync_ShouldMapAndCallRepo()
        {
            var dtoList = new List<PropertyBulkDto>
            {
                new PropertyBulkDto { Name = "Bulk1", Price = 1000 }
            };

            var mapped = new Property { Name = "Bulk1", Price = 1000 };
            _mapperMock.Setup(m => m.Map<Property>(It.IsAny<PropertyBulkDto>())).Returns(mapped);

            await _service.BulkUpsertAsync(dtoList);

            _repoMock.Verify(r => r.BulkUpsertAsync(It.Is<List<Property>>(l => l.Count == 1)), Times.Once);
        }
    }
}
