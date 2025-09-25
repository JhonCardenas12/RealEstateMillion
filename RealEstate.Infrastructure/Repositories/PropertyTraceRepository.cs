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
    public class PropertyTraceRepository : IPropertyTraceRepository
    {
        private readonly IDapperContext _context;
        public PropertyTraceRepository(IDapperContext context) => _context = context;

        public async Task<Guid> AddAsync(PropertyTrace trace)
        {
            var p = new DynamicParameters();
            p.Add("@IdPropertyTrace", dbType: DbType.Guid, direction: ParameterDirection.Output);
            p.Add("@IdProperty", trace.IdProperty);
            p.Add("@DateSale", trace.DateSale);
            p.Add("@Name", trace.Name);
            p.Add("@Value", trace.Value);
            p.Add("@Tax", trace.Tax);
            p.Add("@TraceType", trace.TraceType);
            p.Add("@Notes", trace.Notes);
            await _context.ExecuteAsync("sp_AddPropertyTrace", p, commandType: CommandType.StoredProcedure);
            return p.Get<Guid>("@IdPropertyTrace");
        }

        public async Task DeleteAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdPropertyTrace", id);
            await _context.ExecuteAsync("sp_DeletePropertyTrace", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<PropertyTrace> GetByIdAsync(Guid id)
        {
            var p = new DynamicParameters(); p.Add("@IdPropertyTrace", id);
            return (await _context.QueryAsync<PropertyTrace>("sp_GetPropertyTraceById", p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId)
        {
            var p = new DynamicParameters(); p.Add("@IdProperty", propertyId);
            return await _context.QueryAsync<PropertyTrace>("sp_GetPropertyDetailedById", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(PropertyTrace trace)
        {
            var p = new DynamicParameters();
            p.Add("@IdPropertyTrace", trace.IdPropertyTrace);
            p.Add("@DateSale", trace.DateSale);
            p.Add("@Name", trace.Name);
            p.Add("@Value", trace.Value);
            p.Add("@Tax", trace.Tax);
            p.Add("@TraceType", trace.TraceType);
            p.Add("@Notes", trace.Notes);
            await _context.ExecuteAsync("sp_UpdatePropertyTrace", p, commandType: CommandType.StoredProcedure);
        }
    }
}
