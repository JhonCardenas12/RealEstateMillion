using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces
{
    public interface IOwnerService
    {
        Task<Guid> CreateAsync(OwnerCreateDto dto);
        Task<Owner> GetByIdAsync(Guid id);
        Task<IEnumerable<Owner>> ListAsync(object filter);
        Task UpdateAsync(Guid id, OwnerUpdateDto dto);
        Task DeleteAsync(Guid id);
        Task SetPhotoAsync(Guid id, string fileName, string contentType);
    }
}
