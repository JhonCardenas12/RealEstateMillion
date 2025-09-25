using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public class PropertyImageService : IPropertyImageService
    {
        private readonly IPropertyImageRepository _repo;
        public PropertyImageService(IPropertyImageRepository repo) { _repo = repo; }

        public async Task<Guid> AddAsync(Guid propertyId, string fileName, string contentType, long size)
        {
            var img = new PropertyImage { IdProperty = propertyId, FileName = fileName, ContentType = contentType, Size = size, CreatedAt = DateTime.UtcNow };
            return await _repo.AddAsync(img);
        }

        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(Guid propertyId) => await _repo.GetByPropertyIdAsync(propertyId);
        public async Task<PropertyImage> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);
        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
        public async Task ToggleEnableAsync(Guid id) => await _repo.ToggleEnableAsync(id);
    }
}
