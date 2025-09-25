using System;
namespace RealEstate.Application.DTOs
{
    public class PropertyImageDto
    {
        public Guid IdPropertyImage { get; set; }
        public Guid IdProperty { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
