using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Interfaces
{
    public interface IPropertyImageRepository
    {
        Task<Guid> AddAsync(PropertyImage entity);
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);
        Task<PropertyImage> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ToggleEnableAsync(Guid id);
    }
}
