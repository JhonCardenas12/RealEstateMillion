using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using System.Linq;

namespace RealEstate.Infrastructure.Repositories
{
    public class PropertyImageRepository : IPropertyImageRepository
    {
        private readonly IDapperContext _context;
        public PropertyImageRepository(IDapperContext context) => _context = context;

        public async Task<Guid> AddAsync(PropertyImage entity)
        {
            var p = new DynamicParameters();
            p.Add("@IdPropertyImage", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@IdProperty", entity.IdProperty);
            p.Add("@FileName", entity.FileName);
            p.Add("@ContentType", entity.ContentType);
            p.Add("@Size", entity.Size);
            await _context.ExecuteAsync("sp_AddPropertyImage", p, commandType: CommandType.StoredProcedure);
            return p.Get<Guid>("@IdPropertyImage");
        }

        public async Task DeleteAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdPropertyImage", id);
            await _context.ExecuteAsync("sp_DeletePropertyImage", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<PropertyImage> GetByIdAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdPropertyImage", id);
            return (await _context.QueryAsync<PropertyImage>("sp_GetPropertyImageById", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId)
        {
            var p = new DynamicParameters(); p.Add("@IdProperty", propertyId);
            return await _context.QueryAsync<PropertyImage>("sp_GetPropertyImagesByPropertyId", p, commandType: CommandType.StoredProcedure);
        }

        public async Task ToggleEnableAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdPropertyImage", id);
            await _context.ExecuteAsync("sp_TogglePropertyImageEnabled", p, commandType: CommandType.StoredProcedure);
        }
    }
}
