using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Interfaces
{
    public interface IPropertyTraceRepository
    {
        Task<Guid> AddAsync(PropertyTrace trace);
        Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId);
        Task<PropertyTrace> GetByIdAsync(Guid id);
        Task UpdateAsync(PropertyTrace trace);
        Task DeleteAsync(Guid id);
    }
}
