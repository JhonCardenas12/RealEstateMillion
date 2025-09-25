using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace RealEstate.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<(string FileName, string ContentType, long Size)> SavePropertyImageAsync(System.Guid propertyId, IFormFile file);
        Task<Stream> GetPropertyImageStreamAsync(string fileName);
    }
}
