using System;
namespace RealEstate.Application.DTOs
{
    public class PropertyTraceDto
    {
        public Guid IdPropertyTrace { get; set; }
        public Guid IdProperty { get; set; }
        public DateTime DateSale { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public string TraceType { get; set; }
        public string Notes { get; set; }
    }
}
