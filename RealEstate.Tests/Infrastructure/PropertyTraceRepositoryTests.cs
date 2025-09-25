using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.Infrastructure
{
    [TestFixture]
    public class PropertyTraceRepositoryTests
    {
        private Mock<IDapperContext> _dapperMock;
        private PropertyTraceRepository _repo;

        [SetUp]
        public void Setup()
        {
            _dapperMock = new Mock<IDapperContext>();
            _repo = new PropertyTraceRepository(_dapperMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldReturnGeneratedId()
        {
            var trace = new PropertyTrace
            {
                IdProperty = Guid.NewGuid(),
                DateSale = DateTime.UtcNow,
                Name = "Venta",
                Value = 1000,
                Tax = 50,
                TraceType = "Sale",
                Notes = "Test"
            };

            var generatedId = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_AddPropertyTrace", It.IsAny<object>(), CommandType.StoredProcedure))
               .Callback<string, object, CommandType?>((_, param, __) =>
               {
                   var dyn = (DynamicParameters)param;
                   dyn.Add("@IdPropertyTrace", generatedId, DbType.Guid, ParameterDirection.Output);
               })
                .ReturnsAsync(1);

            var result = await _repo.AddAsync(trace);

            Assert.That(result, Is.EqualTo(generatedId));
        }

        [Test]
        public async Task DeleteAsync_ShouldCallSp()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_DeletePropertyTrace", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.DeleteAsync(id);

            _dapperMock.Verify(d =>
                d.ExecuteAsync("sp_DeletePropertyTrace", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnTrace()
        {
            var id = Guid.NewGuid();
            var expected = new PropertyTrace { IdPropertyTrace = id, Name = "Venta" };

            _dapperMock
                .Setup(d => d.QueryAsync<PropertyTrace>("sp_GetPropertyTraceById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<PropertyTrace> { expected });

            var result = await _repo.GetByIdAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdPropertyTrace, Is.EqualTo(id));
        }

        [Test]
        public async Task GetByPropertyIdAsync_ShouldReturnTraces()
        {
            var propertyId = Guid.NewGuid();
            var list = new List<PropertyTrace>
            {
                new PropertyTrace { IdPropertyTrace = Guid.NewGuid(), IdProperty = propertyId, Name = "Venta" }
            };

            _dapperMock
                .Setup(d => d.QueryAsync<PropertyTrace>("sp_GetPropertyTracesByPropertyId", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(list);

            var result = await _repo.GetByPropertyIdAsync(propertyId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task UpdateAsync_ShouldCallSp()
        {
            var trace = new PropertyTrace
            {
                IdPropertyTrace = Guid.NewGuid(),
                Name = "Venta Actualizada",
                DateSale = DateTime.UtcNow,
                Value = 2000,
                Tax = 100,
                TraceType = "Update",
                Notes = "Updated"
            };

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_UpdatePropertyTrace", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.UpdateAsync(trace);

            _dapperMock.Verify(d =>
                d.ExecuteAsync("sp_UpdatePropertyTrace", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Once);
        }
    }
}
