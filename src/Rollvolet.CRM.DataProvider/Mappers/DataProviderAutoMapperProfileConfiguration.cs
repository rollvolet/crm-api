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
                .ForMember(dest => dest.DepositInvoices, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Memo, opt => opt.Ignore())
                .ForMember(dest => dest.HonorificPrefixId, opt => opt.Ignore())
                .ForMember(dest => dest.LanguageId, opt => opt.Ignore())
                .ForMember(dest => dest.CountryId, opt => opt.Ignore())
                .ForMember(dest => dest.PostalCodeId, opt => opt.Ignore())
                .PreserveReferences();
            
            CreateMap<Models.Contact, Domain.Models.Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.HonorificPrefixId, opt => opt.Ignore())
                .PreserveReferences();
                
            CreateMap<Models.Building, Domain.Models.Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.HonorificPrefixId, opt => opt.Ignore())
                .PreserveReferences();
                
            CreateMap<Models.Country, Domain.Models.Country>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.HonorificPrefix, Domain.Models.HonorificPrefix>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ComposedId))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Models.HonorificPrefix.DecomposeEntityId(src.Id)))
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => Models.HonorificPrefix.DecomposeLanguageId(src.Id)))
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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ComposedId))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.CustomerRecordId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.TelephoneTypeId, opt => opt.MapFrom(src => src.TelephoneType.Id))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src =>  src.Country.Id))
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
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => ParseDate(src.ExpectedDate)))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => ParseDate(src.RequiredDate)))
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.DepositInvoicesHubs.Select(x => x.DepositInvoice)))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.ExpectedDate != null ? ((DateTime) src.ExpectedDate).ToString("d/MM/yyyy") : null))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate != null ? ((DateTime) src.RequiredDate).ToString("d/MM/yyyy") : null))
                // TODO add reverse mapping for deposit invoices
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.Invoice>()
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.DepositInvoiceHubs.Select(x => x.DepositInvoice)))
                .PreserveReferences()
                .ReverseMap()
                 // TODO add reverse mapping for deposit invoices
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.DepositInvoice>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.MainInvoiceHub != null ? src.MainInvoiceHub.Order : null))
                .PreserveReferences()
                .ReverseMap()
                // TODO add reverse mapping for order
                .PreserveReferences();

            CreateMap<Models.InvoiceSupplement, Domain.Models.InvoiceSupplement>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Deposit, Domain.Models.Deposit>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Employee, Domain.Models.Employee>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.WorkingHour, Domain.Models.WorkingHour>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Payment, Domain.Models.Payment>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.UseValue("BETALING"))
                .PreserveReferences();
        }

        private DateTime? ParseDate(string dateString) 
        {
            if (dateString == null) 
            {
                return null;
            }
            else
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(dateString, "d/MM/yyyy", new CultureInfo("nl-BE"), DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", new CultureInfo("nl-BE"), DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "d/MM/yy", new CultureInfo("nl-BE"), DateTimeStyles.None, out dateTime))
                    return dateTime;
                else 
                    return null;
            }
        }
    }
}
