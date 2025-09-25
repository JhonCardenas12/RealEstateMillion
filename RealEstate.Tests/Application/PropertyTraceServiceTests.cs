using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.Application
{
    [TestFixture]
    public class PropertyTraceServiceTests
    {
        private Mock<IPropertyTraceRepository> _repoMock;
        private PropertyTraceService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IPropertyTraceRepository>();
            _service = new PropertyTraceService(_repoMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldReturnId()
        {
            var id = Guid.NewGuid();
            var trace = new PropertyTrace { IdPropertyTrace = id };

            _repoMock.Setup(r => r.AddAsync(trace)).ReturnsAsync(id);

            var result = await _service.AddAsync(trace);

            Assert.AreEqual(id, result);
            _repoMock.Verify(r => r.AddAsync(trace), Times.Once);
        }

        [Test]
        public async Task GetByPropertyIdAsync_ShouldReturnList()
        {
            var propertyId = Guid.NewGuid();
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace { IdPropertyTrace = Guid.NewGuid(), IdProperty = propertyId }
            };

            _repoMock.Setup(r => r.GetByPropertyIdAsync(propertyId)).ReturnsAsync(traces);

            var result = await _service.GetByPropertyIdAsync(propertyId);

            Assert.AreEqual(1, ((List<PropertyTrace>)result).Count);
            _repoMock.Verify(r => r.GetByPropertyIdAsync(propertyId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnTrace()
        {
            var id = Guid.NewGuid();
            var trace = new PropertyTrace { IdPropertyTrace = id };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(trace);

            var result = await _service.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.AreEqual(id, result.IdPropertyTrace);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldCallRepo()
        {
            var trace = new PropertyTrace { IdPropertyTrace = Guid.NewGuid() };

            await _service.UpdateAsync(trace);

            _repoMock.Verify(r => r.UpdateAsync(trace), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallRepo()
        {
            var id = Guid.NewGuid();

            await _service.DeleteAsync(id);

            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
