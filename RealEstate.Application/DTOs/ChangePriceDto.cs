using System;

namespace RealEstate.Application.DTOs
{
    public class ChangePriceDto
    {
        public decimal NewPrice { get; set; }
        public string Reason { get; set; }
    }
}
