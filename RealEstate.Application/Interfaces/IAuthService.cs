using System;
using System.Threading.Tasks;
using RealEstate.Application.DTOs;
namespace RealEstate.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginDto dto);
        Task<Result<Guid>> RegisterAsync(UserRegisterDto dto);
    }
}
