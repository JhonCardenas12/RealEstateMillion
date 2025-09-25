using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using AutoMapper;

namespace RealEstate.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PropertyService(IPropertyRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(PropertyCreateDto dto)
        {
            if (dto.Price <= 0) throw new ArgumentException("Price must be greater than zero");
            var entity = _mapper.Map<Property>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _uow.BeginTransactionAsync();
            try
            {
                var id = await _repo.AddAsync(entity);
                await _uow.CommitAsync();
                return id;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<Property> GetByIdAsync(Guid id) => await _repo.GetByIdDetailedAsync(id);

        public async Task<IEnumerable<Property>> ListAsync(object filter) => await _repo.GetListAsync(filter);

        public async Task UpdateAsync(Guid id, PropertyUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Property not found");
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
        }

        public async Task ChangePriceAsync(Guid id, decimal newPrice, string reason)
        {
            await _repo.ChangePriceAsync(id, newPrice, reason);
        }

        public async Task BulkUpsertAsync(IEnumerable<PropertyBulkDto> items)
        {
            var list = new List<Property>();
            foreach (var it in items)
            {
                list.Add(_mapper.Map<Property>(it));
            }
            await _repo.BulkUpsertAsync(list);
        }
    }
}
