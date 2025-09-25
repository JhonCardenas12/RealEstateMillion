using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IDapperContext _db;
        public OwnerRepository(IDapperContext db) => _db = db;

        public async Task<Guid> AddAsync(Owner owner)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@Name", owner.Name);
            p.Add("@Address", owner.Address);
            p.Add("@ContactEmail", owner.ContactEmail);
            p.Add("@PhotoFileName", owner.PhotoFileName);
            p.Add("@Birthday", owner.Birthday);
            p.Add("@Phone", owner.Phone);

            await _db.ExecuteAsync("sp_CreateOwner", p, CommandType.StoredProcedure);
            return p.Get<Guid>("@IdOwner");
        }

        public async Task DeleteAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", id);
            await _db.ExecuteAsync("sp_DeleteOwner", p, CommandType.StoredProcedure);
        }

        public async Task<Owner> GetByIdAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", id);
            var result = await _db.QueryAsync<Owner>("sp_GetOwnerById", p, CommandType.StoredProcedure);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Owner>> GetListAsync(object filter)
        {
            var p = new DynamicParameters();
            return await _db.QueryAsync<Owner>("sp_ListOwners", p, CommandType.StoredProcedure);
        }

        public async Task SetPhotoAsync(Guid id, string fileName, string contentType)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", id);
            p.Add("@PhotoFileName", fileName);
            p.Add("@ContentType", contentType);

            await _db.ExecuteAsync("sp_SetOwnerPhoto", p, CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Owner owner)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", owner.IdOwner);
            p.Add("@Name", owner.Name);
            p.Add("@Address", owner.Address);
            p.Add("@ContactEmail", owner.ContactEmail);
            p.Add("@PhotoFileName", owner.PhotoFileName);
            p.Add("@Birthday", owner.Birthday);
            p.Add("@Phone", owner.Phone);

            await _db.ExecuteAsync("sp_UpdateOwner", p, CommandType.StoredProcedure);
        }
    }
}
