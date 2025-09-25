using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    public UsersController(IUserService service) { _service = service; }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) { var u = await _service.GetByIdAsync(id); if(u==null) return NotFound(); return Ok(u); }
}
