using RealEstate.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RealEstate.Infrastructure
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _imagesFolder;
        public LocalFileStorageService(IConfiguration config)
        {
            _imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), config.GetValue<string>("FileStorage:ImagesFolder") ?? "Data/Images");
            Directory.CreateDirectory(_imagesFolder);
        }

        public async Task<(string FileName, string ContentType, long Size)> SavePropertyImageAsync(Guid propertyId, IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("File is empty");
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{propertyId:D}_{Guid.NewGuid()}{ext}";
            var full = Path.Combine(_imagesFolder, fileName);
            using (var stream = new FileStream(full, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return (fileName, file.ContentType, file.Length);
        }

        public Task<Stream> GetPropertyImageStreamAsync(string fileName)
        {
            var full = Path.Combine(_imagesFolder, fileName);
            if (!File.Exists(full)) return Task.FromResult<Stream>(null);
            Stream s = File.OpenRead(full);
            return Task.FromResult(s);
        }
    }
}
