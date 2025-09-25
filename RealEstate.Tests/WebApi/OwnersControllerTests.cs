using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using AutoMapper;

namespace RealEstate.Tests.WebApi
{
    [TestFixture]
    public class OwnersControllerTests
    {
        private Mock<IOwnerService> _serviceMock;
        private Mock<IMapper> _mapperMock;
        private OwnersController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IOwnerService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new OwnersController(_serviceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Get_ShouldReturnListOfOwners()
        {
            // Arrange
            var owners = new List<Owner> { new Owner { IdOwner = Guid.NewGuid(), Name = "Test Owner" } };
            _serviceMock.Setup(s => s.ListAsync(null)).ReturnsAsync(owners);

            // Act
            var result = await _controller.Get();

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(owners, ok.Value);
        }

        [Test]
        public async Task GetById_ShouldReturnOwner_WhenExists()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new Owner { IdOwner = ownerId, Name = "Test Owner" };
            _serviceMock.Setup(s => s.GetByIdAsync(ownerId)).ReturnsAsync(owner);

            // Act
            var result = await _controller.Get(ownerId);

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(owner, ok.Value);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenOwnerDoesNotExist()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(ownerId)).ReturnsAsync((Owner)null);

            // Act
            var result = await _controller.Get(ownerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var dto = new OwnerCreateDto { Name = "New Owner" };
            var newId = Guid.NewGuid();
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(newId);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            Assert.AreEqual(201, created.StatusCode);
        }

        [Test]
        public async Task Update_ShouldReturnNoContent()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var dto = new OwnerUpdateDto { Name = "Updated Owner" };
            _serviceMock.Setup(s => s.UpdateAsync(ownerId, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(ownerId, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            _serviceMock.Setup(s => s.DeleteAsync(ownerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(ownerId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
