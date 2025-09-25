using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;
namespace RealEstate.Application.Interfaces
{
    public interface IOwnerRepository
    {
        Task<Guid> AddAsync(Owner owner);
        Task UpdateAsync(Owner owner);
        Task DeleteAsync(Guid id);
        Task<Owner> GetByIdAsync(Guid id);
        Task<IEnumerable<Owner>> GetListAsync(object filter);
        Task SetPhotoAsync(Guid id, string fileName, string contentType);
    }
}
