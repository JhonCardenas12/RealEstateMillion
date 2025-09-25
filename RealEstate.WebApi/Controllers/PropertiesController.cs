using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _service;
    private readonly IMapper _mapper;

    public PropertiesController(IPropertyService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PropertyListFilterDto filter)
    {
        var result = await _service.ListAsync(filter == null ? null : (object)filter);
        var dtos = _mapper.Map<IEnumerable<PropertyDto>>(result);
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var property = await _service.GetByIdAsync(id);
        if (property == null) return NotFound();
        var dto = _mapper.Map<PropertyDetailDto>(property);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PropertyCreateDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PropertyUpdateDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id:guid}/price")]
    public async Task<IActionResult> ChangePrice(Guid id, [FromBody] ChangePriceDto dto)
    {
        await _service.ChangePriceAsync(id, dto.NewPrice, dto.Reason);
        return NoContent();
    }
}
