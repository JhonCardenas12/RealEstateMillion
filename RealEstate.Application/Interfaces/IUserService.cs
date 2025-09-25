using System;
using System.Threading.Tasks;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Interfaces
{
    public interface IUserService
    {
        Task<Guid> CreateUserAsync(UserRegisterDto dto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
        Task<UserDto> GetByIdAsync(Guid id);
    }
}
