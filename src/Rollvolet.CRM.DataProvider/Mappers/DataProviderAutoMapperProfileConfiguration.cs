using System.Linq;
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

            CreateMap<Models.HonorificPrefix, Domain.Models.HonorificPrefix>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => $"{src.Id}-{src.LanguageId}"))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Split('-').FirstOrDefault()))
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Id.Split('-').LastOrDefault()))
                .PreserveReferences();
                
            CreateMap<Models.Language, Domain.Models.Language>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
                
            CreateMap<Models.PostalCode, Domain.Models.PostalCode>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Telephone, Domain.Models.Telephone>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => $"{src.CustomerId}-{src.TelephoneTypeId}-{src.CountryId}-{src.Area}-{src.Number}"))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id.Split('-')[0]))
                .ForMember(dest => dest.TelephoneTypeId, opt => opt.MapFrom(src => src.Id.Split('-')[1]))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Id.Split('-')[2]))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Id.Split('-')[3]))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Id.Split('-')[4]))
                .PreserveReferences();
                
            CreateMap<Models.TelephoneType, Domain.Models.TelephoneType>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
        }
    }
}
