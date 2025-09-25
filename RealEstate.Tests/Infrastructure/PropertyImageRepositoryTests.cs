using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Repositories;

namespace RealEstate.Tests.Infrastructure
{
    [TestFixture]
    public class PropertyImageRepositoryTests
    {
        private Mock<IDapperContext> _dapperMock;
        private PropertyImageRepository _repo;

        [SetUp]
        public void Setup()
        {
            _dapperMock = new Mock<IDapperContext>();
            _repo = new PropertyImageRepository(_dapperMock.Object);
        }
        [Test]
        public async Task AddAsync_ShouldReturnGuidFromOutputParameter()
        {
            // Arrange
            var entity = new PropertyImage
            {
                IdProperty = Guid.NewGuid(),
                FileName = "photo.png",
                ContentType = "image/png",
                Size = 500
            };

            var generatedId = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync(
                    "sp_AddPropertyImage",
                    It.IsAny<object>(),
                    CommandType.StoredProcedure))
                .Callback<string, object, CommandType?>((_, param, __) =>
                {
                    var dyn = (DynamicParameters)param;
                    dyn.Add("@IdPropertyImage", generatedId, DbType.Guid, ParameterDirection.Output);
                })
                .ReturnsAsync(1);

            // Act
            var result = await _repo.AddAsync(entity);

            // Assert
            Assert.That(result, Is.EqualTo(generatedId));
        }


        [Test]
        public async Task DeleteAsync_ShouldCallStoredProcedure()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_DeletePropertyImage", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.DeleteAsync(id);

            _dapperMock.Verify(d => d.ExecuteAsync("sp_DeletePropertyImage", It.IsAny<object>(), CommandType.StoredProcedure), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnPropertyImage()
        {
            var id = Guid.NewGuid();
            var expected = new PropertyImage { IdPropertyImage = id, FileName = "photo.png" };

            _dapperMock
                .Setup(d => d.QueryAsync<PropertyImage>("sp_GetPropertyImageById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<PropertyImage> { expected });

            var result = await _repo.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual("photo.png", result.FileName);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.QueryAsync<PropertyImage>("sp_GetPropertyImageById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<PropertyImage>());

            var result = await _repo.GetByIdAsync(id);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByPropertyIdAsync_ShouldReturnList()
        {
            var propertyId = Guid.NewGuid();
            var list = new List<PropertyImage>
            {
                new PropertyImage { IdPropertyImage = Guid.NewGuid(), FileName = "a.png" },
                new PropertyImage { IdPropertyImage = Guid.NewGuid(), FileName = "b.png" }
            };

            _dapperMock
                .Setup(d => d.QueryAsync<PropertyImage>("sp_GetPropertyImagesByPropertyId", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(list);

            var result = await _repo.GetByPropertyIdAsync(propertyId);

            Assert.IsNotNull(result);
            CollectionAssert.AreEquivalent(list, result);
        }

        [Test]
        public async Task ToggleEnableAsync_ShouldCallStoredProcedure()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_TogglePropertyImageEnabled", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.ToggleEnableAsync(id);

            _dapperMock.Verify(d => d.ExecuteAsync("sp_TogglePropertyImageEnabled", It.IsAny<object>(), CommandType.StoredProcedure), Times.Once);
        }
    }
}
