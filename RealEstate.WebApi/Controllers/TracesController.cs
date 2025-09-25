using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;
using AutoMapper;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

[ApiController]
[Route("api/properties/{propertyId:guid}/[controller]")]
public class TracesController : ControllerBase
{
    private readonly IPropertyTraceService _service;
    private readonly IMapper _mapper;
    public TracesController(IPropertyTraceService service, IMapper mapper) { _service = service; _mapper = mapper; }

    [HttpGet]
    public async Task<IActionResult> Get(Guid propertyId) => Ok(await _service.GetByPropertyIdAsync(propertyId));

    [HttpPost]
    public async Task<IActionResult> Create(Guid propertyId, [FromBody] PropertyTraceCreateDto dto)
    {
        var entity = _mapper.Map<PropertyTrace>(dto);
        entity.IdProperty = propertyId;
        var id = await _service.AddAsync(entity);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpPut("{traceId:guid}")]
    public async Task<IActionResult> Update(Guid propertyId, Guid traceId, [FromBody] PropertyTraceUpdateDto dto)
    {
        var existing = await _service.GetByIdAsync(traceId);
        if (existing == null) return NotFound();
        _mapper.Map(dto, existing);
        await _service.UpdateAsync(existing);
        return NoContent();
    }
}
