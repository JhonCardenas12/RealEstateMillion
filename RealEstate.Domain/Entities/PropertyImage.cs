using System;
namespace RealEstate.Domain.Entities
{
    public class PropertyImage
    {
        public Guid IdPropertyImage { get; set; }
        public Guid IdProperty { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
