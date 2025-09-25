using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Tests.WebApi
{
    [TestFixture]
    public class PropertiesControllerTests
    {
        private Mock<IPropertyService> _serviceMock;
        private Mock<IMapper> _mapperMock;
        private PropertiesController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IPropertyService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new PropertiesController(_serviceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Get_ShouldReturnMappedPropertyDtos()
        {
            // Arrange
            var properties = new List<Property> { new Property { IdProperty = Guid.NewGuid(), Name = "House 1" } };
            var propertyDtos = new List<PropertyDto> { new PropertyDto { Name = "House 1" } };

            _serviceMock.Setup(s => s.ListAsync(It.IsAny<object>())).ReturnsAsync(properties);
            _mapperMock.Setup(m => m.Map<IEnumerable<PropertyDto>>(properties)).Returns(propertyDtos);

            // Act
            var result = await _controller.Get(null);

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(propertyDtos, ok.Value);
        }

        [Test]
        public async Task GetById_ShouldReturnPropertyDetailDto_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var property = new Property { IdProperty = id, Name = "Villa" };
            var propertyDto = new PropertyDetailDto { Name = "Villa" };

            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(property);
            _mapperMock.Setup(m => m.Map<PropertyDetailDto>(property)).Returns(propertyDto);

            // Act
            var result = await _controller.Get(id);

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(propertyDto, ok.Value);
        }

        [Test]
        public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((Property)null);

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WithNewId()
        {
            // Arrange
            var dto = new PropertyCreateDto { Name = "New House", Price = 100000 };
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
            var id = Guid.NewGuid();
            var dto = new PropertyUpdateDto { Name = "Updated House", Price = 200000 };

            _serviceMock.Setup(s => s.UpdateAsync(id, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task ChangePrice_ShouldReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new ChangePriceDto { NewPrice = 250000, Reason = "Market update" };

            _serviceMock.Setup(s => s.ChangePriceAsync(id, dto.NewPrice, dto.Reason)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangePrice(id, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
