using System;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace RealEstate.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<Result<Guid>> RegisterAsync(UserRegisterDto dto)
        {
            var existing = await _userRepo.GetByUsernameAsync(dto.Username);
            if (existing != null) return new Result<Guid> { Success = false, Errors = new List<string>{"Username already exists"} };
            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new AppUser { Username = dto.Username, PasswordHash = hashed, FullName = dto.FullName, Role = dto.Role ?? "User" };
            var id = await _userRepo.CreateUserAsync(user);
            return new Result<Guid>{ Success = true, Value = id };
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            // generate JWT
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expires = jwtSection.GetValue<int>("ExpiresMinutes");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(key);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expires > 0 ? expires : 60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthResponseDto { Token = tokenHandler.WriteToken(token), ExpiresIn = expires };
        }
    }
}
