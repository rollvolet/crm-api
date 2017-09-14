using AutoMapper;

namespace Rollvolet.CRM.DataProvider.Mappers
{
    public class DataProviderAutoMapperProfileConfiguration : Profile
    {
        public DataProviderAutoMapperProfileConfiguration()
        {
            CreateMap<Models.Customer, Domain.Models.Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Number))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
            
            CreateMap<Models.Contact, Domain.Models.Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
                
            CreateMap<Models.Building, Domain.Models.Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
                
            CreateMap<Models.Country, Domain.Models.Country>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
        }
    }
}
