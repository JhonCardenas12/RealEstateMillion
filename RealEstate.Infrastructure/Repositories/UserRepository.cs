using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDapperContext _context;
        public UserRepository(IDapperContext context) => _context = context;

        public async Task<Guid> CreateUserAsync(AppUser user)
        {
            var p = new DynamicParameters();
            p.Add("@IdUser", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@Username", user.Username);
            p.Add("@PasswordHash", user.PasswordHash);
            p.Add("@FullName", user.FullName);
            p.Add("@Role", user.Role);
            await _context.ExecuteAsync("sp_CreateUser", p, commandType: CommandType.StoredProcedure);
            return p.Get<Guid>("@IdUser");
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            var p = new DynamicParameters(); p.Add("@Username", username);
            return (await _context.QueryAsync<AppUser>("sp_GetUserByUsername", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<AppUser> GetByIdAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdUser", id);
            return (await _context.QueryAsync<AppUser>("sp_GetUserById", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
    }
}
