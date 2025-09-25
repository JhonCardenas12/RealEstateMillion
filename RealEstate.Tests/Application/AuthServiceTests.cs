using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using RealEstate.Application.Services;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RealEstate.Tests.Application
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private IConfiguration _config;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();

            // Fake config
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Key", "change_this_secret_to_strong_key"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpiresMinutes", "60"}
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(_userRepoMock.Object, _config);
        }

        [Test]
        public async Task RegisterAsync_ShouldFail_WhenUsernameAlreadyExists()
        {
            // Arrange
            var dto = new UserRegisterDto { Username = "testuser", Password = "123456" };
            _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser"))
                .ReturnsAsync(new AppUser { Username = "testuser" });

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.That(result.Errors, Does.Contain("Username already exists"));
        }

        [Test]
        public async Task RegisterAsync_ShouldCreateUser_WhenUsernameNotExists()
        {
            // Arrange
            var dto = new UserRegisterDto { Username = "newuser", Password = "123456", FullName = "New User" };
            _userRepoMock.Setup(r => r.GetByUsernameAsync("newuser"))
                .ReturnsAsync((AppUser)null);
            _userRepoMock.Setup(r => r.CreateUserAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _authService.RegisterAsync(dto);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreNotEqual(Guid.Empty, result.Value);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var dto = new UserLoginDto { Username = "nouser", Password = "123" };
            _userRepoMock.Setup(r => r.GetByUsernameAsync("nouser"))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordInvalid()
        {
            // Arrange
            var dto = new UserLoginDto { Username = "testuser", Password = "wrongpass" };
            var hashed = BCrypt.Net.BCrypt.HashPassword("correctpass");
            _userRepoMock.Setup(r => r.GetByUsernameAsync("testuser"))
                .ReturnsAsync(new AppUser { IdUser = Guid.NewGuid(), Username = "testuser", PasswordHash = hashed });

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            // Arrange
            var dto = new UserLoginDto { Username = "validuser", Password = "123456" };
            var hashed = BCrypt.Net.BCrypt.HashPassword("123456");
            var user = new AppUser
            {
                IdUser = Guid.NewGuid(),
                Username = "validuser",
                PasswordHash = hashed,
                Role = "Admin"
            };
            _userRepoMock.Setup(r => r.GetByUsernameAsync("validuser"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Token);
            Assert.That(result.Token, Does.Contain("."));
            Assert.AreEqual(60, result.ExpiresIn);
        }
    }
}
