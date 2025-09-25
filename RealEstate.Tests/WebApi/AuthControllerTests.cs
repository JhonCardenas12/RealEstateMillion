using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace RealEstate.Tests.WebApi
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Test]
        public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var dto = new UserRegisterDto { Username = "jhon", Password = "123", FullName = "Jhon Doe" };
            var expectedId = Guid.NewGuid();

            _authServiceMock
                .Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(new Result<Guid> { Success = true, Value = expectedId });

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedId, okResult.Value);
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var dto = new UserRegisterDto { Username = "jhon", Password = "123" };
            var errors = new List<string> { "Username already exists" };

            _authServiceMock
                .Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(new Result<Guid> { Success = false, Errors = errors });

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
            Assert.AreEqual(errors, badRequest.Value);
        }

        [Test]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new UserLoginDto { Username = "jhon", Password = "123" };
            var authResponse = new AuthResponseDto { Token = "jwt-token", ExpiresIn = 60 };

            _authServiceMock
                .Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(authResponse, okResult.Value);
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var dto = new UserLoginDto { Username = "jhon", Password = "wrong" };

            _authServiceMock
                .Setup(x => x.LoginAsync(dto))
                .ReturnsAsync((AuthResponseDto)null);

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorized = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual(401, unauthorized.StatusCode);
        }
    }
}
