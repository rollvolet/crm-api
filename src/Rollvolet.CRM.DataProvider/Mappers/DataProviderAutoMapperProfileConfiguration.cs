using System;
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
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
            
            CreateMap<Models.Contact, Domain.Models.Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
                
            CreateMap<Models.Building, Domain.Models.Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None).FirstOrDefault()))
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None).LastOrDefault()))
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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => $"{src.CustomerRecordId}-{src.TelephoneTypeId}-{src.CountryId}-{src.Area}-{src.Number}"))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.CustomerRecordId, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None)[0]))
                .ForMember(dest => dest.TelephoneTypeId, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None)[1]))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None)[2]))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None)[3]))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Id.Split('-', StringSplitOptions.None)[4]))
                .PreserveReferences();
                
            CreateMap<Models.TelephoneType, Domain.Models.TelephoneType>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Request, Domain.Models.Request>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.WayOfEntry, Domain.Models.WayOfEntry>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Visit, Domain.Models.Visit>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();
        }
    }
}
