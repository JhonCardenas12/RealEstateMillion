using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IPropertyImageService
    {
        Task<Guid> AddAsync(Guid propertyId, string fileName, string contentType, long size);
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId);
        Task<PropertyImage> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ToggleEnableAsync(Guid id);
    }
}
