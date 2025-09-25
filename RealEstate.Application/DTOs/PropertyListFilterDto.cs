using System;

namespace RealEstate.Application.DTOs
{
    public class PropertyListFilterDto
    {
        public string Name { get; set; }
        public string CodeInternal { get; set; }
        public string Address { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public Guid? IdOwner { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
