using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
using System.Data;
using Dapper;
using RealEstate.Application.Interfaces;
using System.Linq;

namespace RealEstate.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IDapperContext _context;
        public PropertyRepository(IDapperContext context) => _context = context;
        public async Task<Guid> AddAsync(Property entity)
        {
            var p = new DynamicParameters();
            p.Add("@IdProperty", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@Name", entity.Name);
            p.Add("@CodeInternal", entity.CodeInternal);
            p.Add("@Address", entity.Address);
            p.Add("@Price", entity.Price);
            p.Add("@Year", entity.Year);
            p.Add("@IdOwner", entity.IdOwner);
            p.Add("@Description", entity.Description);
            p.Add("@Bedrooms", entity.Bedrooms);
            p.Add("@Bathrooms", entity.Bathrooms);
            p.Add("@SquareMeters", entity.SquareMeters);
            await _context.ExecuteAsync("sp_CreateProperty", p, commandType: CommandType.StoredProcedure);
            return p.Get<Guid>("@IdProperty");
        }

        public async Task ChangePriceAsync(Guid id, decimal newPrice, string reason)
        {
            var p = new DynamicParameters();
            p.Add("@IdProperty", id);
            p.Add("@NewPrice", newPrice);
            p.Add("@Reason", reason);
            await _context.ExecuteAsync("sp_ChangePropertyPrice", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@IdProperty", id);
            await _context.ExecuteAsync("sp_DeleteProperty", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<Property> GetByIdAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@IdProperty", id);
            return (await _context.QueryAsync<Property>("sp_GetPropertyById", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<Property> GetByIdDetailedAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@IdProperty", id);
            var prop = (await _context.QueryAsync<Property>("sp_GetPropertyDetailedById", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (prop == null) return null;
            return prop;
        }

        public async Task<IEnumerable<Property>> GetListAsync(object filter)
        {
            var p = new DynamicParameters();
            if (filter != null && filter is System.Collections.IDictionary dict)
            {
                if (dict.Contains("Name")) p.Add("@Name", dict["Name"].ToString());
                if (dict.Contains("MinPrice")) p.Add("@MinPrice", dict["MinPrice"]);
                if (dict.Contains("MaxPrice")) p.Add("@MaxPrice", dict["MaxPrice"]);
                if (dict.Contains("IdOwner")) p.Add("@IdOwner", dict["IdOwner"]);
            }
            return await _context.QueryAsync<Property>("sp_ListProperties", p, commandType: CommandType.StoredProcedure);
        }

        public async Task BulkUpsertAsync(IEnumerable<Property> properties)
        {
            foreach (var pItem in properties)
            {
                var p = new DynamicParameters();
                p.Add("@CodeInternal", pItem.CodeInternal);
                p.Add("@Name", pItem.Name);
                p.Add("@Address", pItem.Address);
                p.Add("@Price", pItem.Price);
                p.Add("@Year", pItem.Year);
                p.Add("@IdOwner", pItem.IdOwner);
                p.Add("@Description", pItem.Description);
                p.Add("@Bedrooms", pItem.Bedrooms);
                p.Add("@Bathrooms", pItem.Bathrooms);
                p.Add("@SquareMeters", pItem.SquareMeters);
                await _context.ExecuteAsync("sp_BulkUpsertProperty", p, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
        {
            var p = new DynamicParameters();
            p.Add("@IdOwner", ownerId);
            return await _context.QueryAsync<Property>("sp_GetPropertiesByOwnerId", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Property entity)
        {
            var parameters = new
            {
                entity.IdProperty,
                entity.Name,
                entity.CodeInternal,
                entity.Address,
                entity.Price,
                entity.Year,
                entity.IdOwner,
                entity.Description,
                entity.Bedrooms,
                entity.Bathrooms,
                entity.SquareMeters
            };

            await _context.ExecuteAsync("st_UpdateProperty", parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
