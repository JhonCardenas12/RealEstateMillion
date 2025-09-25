using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using RealEstate.Application.Interfaces;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/properties/{propertyId:guid}/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IFileStorageService _files;
    private readonly IPropertyImageService _service;
    public ImagesController(IFileStorageService files, IPropertyImageService service) { _files = files; _service = service; }

    [HttpPost]
    public async Task<IActionResult> Upload(Guid propertyId, IFormFile file)
    {
        var saved = await _files.SavePropertyImageAsync(propertyId, file);
        var id = await _service.AddAsync(propertyId, saved.FileName, saved.ContentType, saved.Size);
        return CreatedAtAction(null, new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> List(Guid propertyId) => Ok(await _service.GetByPropertyIdAsync(propertyId));

    [HttpGet("/api/properties/images/{imageId:guid}")]
    public async Task<IActionResult> Download(Guid imageId)
    {
        var meta = await _service.GetByIdAsync(imageId);
        if (meta == null) return NotFound();
        var stream = await _files.GetPropertyImageStreamAsync(meta.FileName);
        return File(stream, meta.ContentType, meta.FileName);
    }

    [HttpDelete("/api/properties/images/{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid imageId) { await _service.DeleteAsync(imageId); return NoContent(); }
}
