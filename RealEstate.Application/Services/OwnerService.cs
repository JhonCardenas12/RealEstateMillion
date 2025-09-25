using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _repo;
        private readonly IUnitOfWork _uow;
        public OwnerService(IOwnerRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

        public async Task<Guid> CreateAsync(OwnerCreateDto dto)
        {
            var owner = new Owner { Name = dto.Name, Address = dto.Address, ContactEmail = dto.ContactEmail, PhotoFileName = dto.PhotoFileName, Birthday = dto.Birthday, Phone = dto.Phone };
            await _uow.BeginTransactionAsync();
            try { var id = await _repo.AddAsync(owner); await _uow.CommitAsync(); return id; } catch { await _uow.RollbackAsync(); throw; }
        }

        public async Task<Owner> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);
        public async Task<IEnumerable<Owner>> ListAsync(object filter) => await _repo.GetListAsync(filter);
        public async Task UpdateAsync(Guid id, OwnerUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Owner not found");
            existing.Name = dto.Name; existing.Address = dto.Address; existing.ContactEmail = dto.ContactEmail; existing.PhotoFileName = dto.PhotoFileName; existing.Birthday = dto.Birthday; existing.Phone = dto.Phone;
            await _repo.UpdateAsync(existing);
        }
        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
        public async Task SetPhotoAsync(Guid id, string fileName, string contentType) => await _repo.SetPhotoAsync(id, fileName, contentType);
    }
}
