using AutoMapper;

namespace Rollvolet.CRM.DataProvider.Mappers
{
    public class DataProviderAutoMapperProfileConfiguration : Profile
    {
        public DataProviderAutoMapperProfileConfiguration()
        {
            CreateMap<Models.Customer, Domain.Models.Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerId))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
            
            CreateMap<Models.Contact, Domain.Models.Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.RelativeId, opt => opt.MapFrom(src => src.ContactId))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
                
            CreateMap<Models.Building, Domain.Models.Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.RelativeId, opt => opt.MapFrom(src => src.BuildingId))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
        }
    }
}
