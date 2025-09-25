using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<Guid> CreateAsync(PropertyCreateDto dto);
        Task<Property> GetByIdAsync(Guid id);
        Task<IEnumerable<Property>> ListAsync(object filter);
        Task UpdateAsync(Guid id, PropertyUpdateDto dto);
        Task ChangePriceAsync(Guid id, decimal newPrice, string reason);
        Task BulkUpsertAsync(IEnumerable<PropertyBulkDto> items);
    }
}
