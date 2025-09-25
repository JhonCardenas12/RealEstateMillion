using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Repositories;

namespace RealEstate.Tests.Infrastructure.Repositories
{
    [TestFixture]
    public class OwnerRepositoryTests
    {
        private Mock<IDapperContext> _dapperMock;
        private OwnerRepository _repo;

        [SetUp]
        public void Setup()
        {
            _dapperMock = new Mock<IDapperContext>();
            _repo = new OwnerRepository(_dapperMock.Object);
        }


        [Test]
        public async Task DeleteAsync_ShouldCallStoredProcedure()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_DeleteOwner", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.DeleteAsync(id);

            _dapperMock.Verify(d => d.ExecuteAsync("sp_DeleteOwner", It.IsAny<object>(), CommandType.StoredProcedure), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnOwner()
        {
            var id = Guid.NewGuid();
            var expected = new Owner { IdOwner = id, Name = "John Doe" };

            _dapperMock
                .Setup(d => d.QueryAsync<Owner>("sp_GetOwnerById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<Owner> { expected });

            var result = await _repo.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var id = Guid.NewGuid();

            _dapperMock
                .Setup(d => d.QueryAsync<Owner>("sp_GetOwnerById", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(new List<Owner>());

            var result = await _repo.GetByIdAsync(id);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetListAsync_ShouldReturnMultipleOwners()
        {
            var list = new List<Owner>
            {
                new Owner { IdOwner = Guid.NewGuid(), Name = "John" },
                new Owner { IdOwner = Guid.NewGuid(), Name = "Jane" }
            };

            _dapperMock
                .Setup(d => d.QueryAsync<Owner>("sp_ListOwners", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(list);

            var result = await _repo.GetListAsync(null);

            Assert.IsNotNull(result);
            CollectionAssert.AreEquivalent(list, result);
        }

        [Test]
        public async Task SetPhotoAsync_ShouldCallStoredProcedure()
        {
            var id = Guid.NewGuid();
            var fileName = "photo.png";
            var contentType = "image/png";

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_SetOwnerPhoto", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.SetPhotoAsync(id, fileName, contentType);

            _dapperMock.Verify(d => d.ExecuteAsync("sp_SetOwnerPhoto", It.IsAny<object>(), CommandType.StoredProcedure), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldCallStoredProcedure()
        {
            var owner = new Owner
            {
                IdOwner = Guid.NewGuid(),
                Name = "Updated",
                Address = "New Address",
                ContactEmail = "test@test.com",
                PhotoFileName = "file.jpg",
                Phone = "12345"
            };

            _dapperMock
                .Setup(d => d.ExecuteAsync("sp_UpdateOwner", It.IsAny<object>(), CommandType.StoredProcedure))
                .ReturnsAsync(1);

            await _repo.UpdateAsync(owner);

            _dapperMock.Verify(d => d.ExecuteAsync("sp_UpdateOwner", It.IsAny<object>(), CommandType.StoredProcedure), Times.Once);
        }
    }
}
