using System;
namespace RealEstate.Application.DTOs
{
    public class OwnerDto
    {
        public Guid IdOwner { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public string PhotoFileName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone
        {
            get; set;
        }
    }
}

