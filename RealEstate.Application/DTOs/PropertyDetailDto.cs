using System;
using System.Collections.Generic;
namespace RealEstate.Application.DTOs
{
    public class PropertyDetailDto : PropertyDto
    {
        public OwnerDto Owner { get; set; }
        public IEnumerable<PropertyImageDto> Images { get; set; }
        public IEnumerable<PropertyTraceDto> Traces { get; set; }
    }
}
