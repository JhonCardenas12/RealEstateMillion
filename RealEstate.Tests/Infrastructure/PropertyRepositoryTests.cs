using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Application.Interfaces;
using System.Linq;

namespace RealEstate.Tests.Infrastructure
{
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private Mock<IDapperContext> _dapperMock;
        private PropertyRepository _repo;

        [SetUp]
        public void Setup()
        {
            _dapperMock = new Mock<IDapperContext>();
            _repo = new PropertyRepository(_dapperMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldReturnGeneratedId()
        {
            var property = new Property
            {
                Name = "Casa",
                CodeInternal = "C001",
                Address = "Street 1",
                Price = 1000,
                Year = 2022,
                IdOwner = Guid.NewGuid(),
                Description = "Nice house",
                Bedrooms = 3,
                Bathrooms = 2,
                SquareMeters = 120
            };

            var generatedId = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_CreateProperty", It.IsAny<object>(), CommandType.StoredProcedure))
                .Callback<string, object, CommandType?>((_, param, __) =>
                {
                    var dyn = (DynamicParameters)param;
                    dyn.Add("@IdProperty", generatedId, DbType.Guid, ParameterDirection.Output);
                })
                .ReturnsAsync(1);

            var result = await _repo.AddAsync(property);

            Assert.That(result, Is.EqualTo(generatedId));
        }

        [Test]
        public async Task ChangePriceAsync_ShouldCallSp()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_ChangePropertyPrice", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.ChangePriceAsync(id, 2000, "Test reason");

            _dapperMock.Verify(d =>
                d.ExecuteAsync("sp_ChangePropertyPrice", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldCallSp()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_DeleteProperty", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.DeleteAsync(id);

            _dapperMock.Verify(d =>
                d.ExecuteAsync("sp_DeleteProperty", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProperty()
        {
            var id = Guid.NewGuid();
            var expected = new Property { IdProperty = id, Name = "Casa" };

            _dapperMock
                .Setup(d => d.QueryAsync<Property>("sp_GetPropertyById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<Property> { expected });

            var result = await _repo.GetByIdAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdProperty, Is.EqualTo(id));
        }

        [Test]
        public async Task GetByIdDetailedAsync_ShouldReturnProperty()
        {
            var id = Guid.NewGuid();
            var expected = new Property { IdProperty = id, Name = "Casa" };

            _dapperMock
                .Setup(d => d.QueryAsync<Property>("sp_GetPropertyDetailedById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<Property> { expected });

            var result = await _repo.GetByIdDetailedAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdProperty, Is.EqualTo(id));
        }

        [Test]
        public async Task GetListAsync_ShouldReturnProperties()
        {
            var list = new List<Property>
            {
                new Property { IdProperty = Guid.NewGuid(), Name = "Casa 1" },
                new Property { IdProperty = Guid.NewGuid(), Name = "Casa 2" }
            };

            _dapperMock
                .Setup(d => d.QueryAsync<Property>("sp_ListProperties", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(list);

            var result = await _repo.GetListAsync(null);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task BulkUpsertAsync_ShouldCallSpForEach()
        {
            var properties = new List<Property>
            {
                new Property { IdProperty = Guid.NewGuid(), Name = "Casa 1", CodeInternal = "C001" },
                new Property { IdProperty = Guid.NewGuid(), Name = "Casa 2", CodeInternal = "C002" }
            };

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_BulkUpsertProperty", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.BulkUpsertAsync(properties);

            _dapperMock.Verify(d =>
                d.ExecuteAsync("sp_BulkUpsertProperty", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Exactly(properties.Count));
        }

        [Test]
        public async Task GetByOwnerIdAsync_ShouldReturnProperties()
        {
            var ownerId = Guid.NewGuid();
            var list = new List<Property>
            {
                new Property { IdProperty = Guid.NewGuid(), IdOwner = ownerId, Name = "Casa 1" }
            };

            _dapperMock
                .Setup(d => d.QueryAsync<Property>("sp_GetPropertiesByOwnerId", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(list);

            var result = await _repo.GetByOwnerIdAsync(ownerId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_ShouldCallSp()
        {
            var property = new Property
            {
                IdProperty = Guid.NewGuid(),
                Name = "Casa Updated",
                CodeInternal = "C001"
            };

            _dapperMock
                .Setup(d => d.ExecuteAsync("st_UpdateProperty", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.UpdateAsync(property);

            _dapperMock.Verify(d =>
                d.ExecuteAsync("st_UpdateProperty", It.IsAny<object>(), CommandType.StoredProcedure),
                Times.Once);
        }
    }
}
