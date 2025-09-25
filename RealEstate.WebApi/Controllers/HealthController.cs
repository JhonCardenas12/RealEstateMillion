using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

[ApiController]
[Route("/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _hc;
    public HealthController(HealthCheckService hc) => _hc = hc;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var r = await _hc.CheckHealthAsync();
        return r.Status == HealthStatus.Healthy ? Ok("Healthy") : StatusCode(503, "Unhealthy");
    }
}
