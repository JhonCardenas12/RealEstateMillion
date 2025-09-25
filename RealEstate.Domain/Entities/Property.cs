using System;
namespace RealEstate.Domain.Entities
{
    public class Property
    {
        public Guid IdProperty { get; set; }
        public string Name { get; set; }
        public string CodeInternal { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public Guid IdOwner { get; set; }
        public string Description { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal SquareMeters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
