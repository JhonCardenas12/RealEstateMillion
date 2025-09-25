using System;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IAuthService _auth;
        public UserService(IUserRepository repo, IAuthService auth) { _repo = repo; _auth = auth; }

        public async Task<Guid> CreateUserAsync(UserRegisterDto dto)
        {
            var result = await _auth.RegisterAsync(dto);
            if (!result.Success) throw new InvalidOperationException(string.Join(';', result.Errors ?? new System.Collections.Generic.List<string>()));
            return result.Value;
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto) => await _auth.LoginAsync(dto);
        public async Task<UserDto> GetByIdAsync(Guid id) { var u = await _repo.GetByIdAsync(id); if (u==null) return null; return new UserDto{ Id = u.IdUser, Username = u.Username, FullName = u.FullName, Role = u.Role }; }
    }
}
