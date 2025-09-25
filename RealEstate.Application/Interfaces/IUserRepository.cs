using System;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(AppUser user);
        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser> GetByIdAsync(Guid id);
    }
}
