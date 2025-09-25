using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        if (!result.Success) return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    [HttpPost("login")]
 
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var token = await _auth.LoginAsync(dto);
        if (token == null) return Unauthorized();
        return Ok(token);
    }
}
