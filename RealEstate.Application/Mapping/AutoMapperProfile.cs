using AutoMapper;
using RealEstate.Domain.Entities;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PropertyCreateDto, Property>();
            CreateMap<PropertyUpdateDto, Property>();
            CreateMap<Property, PropertyDto>();
            CreateMap<Property, PropertyDetailDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<PropertyImage, PropertyImageDto>();
            CreateMap<PropertyTrace, PropertyTraceDto>();
            CreateMap<AppUser, UserDto>().ForMember(d => d.Id, o => o.MapFrom(s => s.IdUser));
            CreateMap<PropertyTraceCreateDto, PropertyTrace>();
            CreateMap<PropertyTraceUpdateDto, PropertyTrace>();
        }
    }
}
