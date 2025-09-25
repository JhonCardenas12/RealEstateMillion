using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Interfaces
{
    public interface IPropertyRepository
    {
        Task<Guid> AddAsync(Property entity);
        Task UpdateAsync(Property entity);
        Task DeleteAsync(Guid id);
        Task<Property> GetByIdAsync(Guid id);
        Task<Property> GetByIdDetailedAsync(Guid id);
        Task<IEnumerable<Property>> GetListAsync(object filter);
        Task ChangePriceAsync(Guid id, decimal newPrice, string reason);
        Task BulkUpsertAsync(IEnumerable<Property> properties);
        Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
    }
}
