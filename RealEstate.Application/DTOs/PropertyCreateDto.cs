using System;
namespace RealEstate.Application.DTOs
{
    public class PropertyCreateDto
    {
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
    }
}
