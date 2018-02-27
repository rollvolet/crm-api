using System;
using System.Globalization;
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
                .ForMember(dest => dest.Memo, opt => opt.MapFrom(src => src.Memo != null ? src.Memo.Text : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CustomerTags.Select(e => e.Tag)))
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

            CreateMap<Models.Tag, Domain.Models.Tag>()
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

            CreateMap<Models.Offer, Domain.Models.Offer>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.SubmissionType, Domain.Models.SubmissionType>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.VatRate, Domain.Models.VatRate>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Order, Domain.Models.Order>()
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.ExpectedDate != null ? DateTime.ParseExact(src.ExpectedDate, "d/MM/yyyy", new CultureInfo("nl-BE")) : (DateTime?) null))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate != null ? DateTime.ParseExact(src.RequiredDate, "d/MM/yyyy", new CultureInfo("nl-BE")) : (DateTime?) null))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.ExpectedDate != null ? ((DateTime) src.ExpectedDate).ToString("d/MM/yyyy") : null))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate != null ? ((DateTime) src.RequiredDate).ToString("d/MM/yyyy") : null))
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.Invoice>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.InvoiceSupplement, Domain.Models.InvoiceSupplement>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Deposit, Domain.Models.Deposit>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Payment, Domain.Models.Payment>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.UseValue("BETALING"))
                .PreserveReferences();
        }
    }
}
