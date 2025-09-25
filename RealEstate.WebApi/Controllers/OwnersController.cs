using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using AutoMapper;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class OwnersController : ControllerBase
{
    private readonly IOwnerService _service;
    private readonly IMapper _mapper;
    public OwnersController(IOwnerService service, IMapper mapper) { _service = service; _mapper = mapper; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.ListAsync(null));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) { var o = await _service.GetByIdAsync(id); if (o==null) return NotFound(); return Ok(o); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OwnerCreateDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OwnerUpdateDto dto) { await _service.UpdateAsync(id, dto); return NoContent(); }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) { await _service.DeleteAsync(id); return NoContent(); }
}
