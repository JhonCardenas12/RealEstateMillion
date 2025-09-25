using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services
{
    public class PropertyTraceService : IPropertyTraceService
    {
        private readonly IPropertyTraceRepository _repo;
        public PropertyTraceService(IPropertyTraceRepository repo) { _repo = repo; }

        public async Task<Guid> AddAsync(PropertyTrace trace) => await _repo.AddAsync(trace);
        public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(Guid propertyId) => await _repo.GetByPropertyIdAsync(propertyId);
        public async Task<PropertyTrace> GetByIdAsync(Guid id) => await _repo.GetByIdAsync(id);
        public async Task UpdateAsync(PropertyTrace trace) => await _repo.UpdateAsync(trace);
        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
    }
}
