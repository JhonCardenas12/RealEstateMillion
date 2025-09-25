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
    public class TracesControllerTests
    {
        private Mock<IPropertyTraceService> _serviceMock;
        private Mock<IMapper> _mapperMock;
        private TracesController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IPropertyTraceService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new TracesController(_serviceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Get_ShouldReturnTraces()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var traces = new List<PropertyTrace> { new PropertyTrace { IdPropertyTrace = Guid.NewGuid() } };
            _serviceMock.Setup(s => s.GetByPropertyIdAsync(propertyId)).ReturnsAsync(traces);

            // Act
            var result = await _controller.Get(propertyId);

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(traces, ok.Value);
        }

        [Test]
        public async Task Create_ShouldReturnCreatedAtAction_WithNewId()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var dto = new PropertyTraceCreateDto { Name = "Trace 1" };
            var entity = new PropertyTrace { IdProperty = propertyId, Name = "Trace 1" };
            var newId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<PropertyTrace>(dto)).Returns(entity);
            _serviceMock.Setup(s => s.AddAsync(entity)).ReturnsAsync(newId);

            // Act
            var result = await _controller.Create(propertyId, dto);

            // Assert
            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            Assert.AreEqual(201, created.StatusCode);
        }

        [Test]
        public async Task Update_ShouldReturnNoContent_WhenTraceExists()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var traceId = Guid.NewGuid();
            var dto = new PropertyTraceUpdateDto { Name = "Updated" };
            var existing = new PropertyTrace { IdPropertyTrace = traceId, Name = "Old" };

            _serviceMock.Setup(s => s.GetByIdAsync(traceId)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(dto, existing));
            _serviceMock.Setup(s => s.UpdateAsync(existing)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(propertyId, traceId, dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Update_ShouldReturnNotFound_WhenTraceDoesNotExist()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var traceId = Guid.NewGuid();
            var dto = new PropertyTraceUpdateDto { Name = "Updated" };

            _serviceMock.Setup(s => s.GetByIdAsync(traceId)).ReturnsAsync((PropertyTrace)null);

            // Act
            var result = await _controller.Update(propertyId, traceId, dto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
