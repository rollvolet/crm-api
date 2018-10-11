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
                .ForMember(dest => dest.VatNumber, opt => opt.MapFrom(src => Domain.Models.Customer.SerializeVatNumber(src.VatNumber)))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .ForMember(dest => dest.Memo, opt => opt.MapFrom(src => src.Memo != null ? src.Memo.Text : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.CustomerTags.Select(e => e.Tag)))
                .ForMember(dest => dest.DepositInvoices, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.Url) ? null : src.Url)) // zero-length DB constraint
                .ForMember(dest => dest.VatNumber, opt => opt.MapFrom(src => Models.Customer.SerializeVatNumber(src.VatNumber)))
                .ForMember(dest => dest.Memo, opt => opt.Ignore())
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country != null ? src.Country.Id : null))
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Language != null ? src.Language.Id : null))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.Ignore())
                .ForMember(dest => dest.HonorificPrefixId, opt =>opt.MapFrom(src => Models.HonorificPrefix.DecomposeEntityId(src.HonorificPrefix.Id)))
                .ForMember(dest => dest.Telephones, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerTags, opt => opt.Ignore())
                .ForMember(dest => dest.Requests, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Contact, Domain.Models.Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country != null ? src.Country.Id : null))
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Language != null ? src.Language.Id : null))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.Ignore())
                .ForMember(dest => dest.HonorificPrefixId, opt =>opt.MapFrom(src => Models.HonorificPrefix.DecomposeEntityId(src.HonorificPrefix.Id)))
                .ForMember(dest => dest.Telephones, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Building, Domain.Models.Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DataId))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.EmbeddedPostalCode))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.EmbeddedCity))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country != null ? src.Country.Id : null))
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src => src.Language != null ? src.Language.Id : null))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.Ignore())
                .ForMember(dest => dest.HonorificPrefixId, opt =>opt.MapFrom(src => Models.HonorificPrefix.DecomposeEntityId(src.HonorificPrefix.Id)))
                .ForMember(dest => dest.Telephones, opt => opt.Ignore())
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
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Domain.Models.Telephone.SerializeNumber(src.Number)))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.CustomerRecordId, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.DataId : (src.Contact != null ? src.Contact.Id : src.Building.Id)))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Models.Telephone.SerializeNumber(src.Number)))
                .ForMember(dest => dest.CustomerRecord, opt => opt.Ignore())
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Country.Id))
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.TelephoneTypeId, opt => opt.MapFrom(src => src.TelephoneType.Id))
                .ForMember(dest => dest.TelephoneType, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.TelephoneType, Domain.Models.TelephoneType>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Telephones, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Tag, Domain.Models.Tag>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.CustomerTags, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Request, Domain.Models.Request>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.WayOfEntry, opt => opt.Ignore())
                .ForMember(dest => dest.WayOfEntryId, opt => opt.MapFrom(src => src.WayOfEntry != null ? src.WayOfEntry.Id : null))
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.Contact != null ? src.Contact.Number : (int?) null))
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.Building != null ? src.Building.Number : (int?) null))
                .ForMember(dest => dest.Visit, opt => opt.Ignore())
                .ForMember(dest => dest.Offer, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.WayOfEntry, Domain.Models.WayOfEntry>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Visit, Domain.Models.Visit>()
                .ForMember(dest => dest.Period, opt => opt.Ignore())
                .ForMember(dest => dest.FromHour, opt => opt.Ignore())
                .ForMember(dest => dest.UntilHour, opt => opt.Ignore())
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Request != null ? src.Request :
                                                        (src.RequestId != null ? new Models.Request() { Id = (int) src.RequestId } : null)))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Request, opt => opt.Ignore())
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Request.Id))
                .PreserveReferences();

            CreateMap<Models.Offer, Domain.Models.Offer>()
                .ForMember(dest => dest.RequestNumber, opt => opt.MapFrom(src => src.RequestId))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.Contact != null ? src.Contact.Number : (int?) null))
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.Building != null ? src.Building.Number : (int?) null))
                .ForMember(dest => dest.SubmissionType, opt => opt.Ignore())
                .ForMember(dest => dest.SubmissionTypeId, opt => opt.MapFrom(src => src.SubmissionType != null ? src.SubmissionType.Id : null))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
                .ForMember(dest => dest.Request, opt => opt.Ignore())
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.Request.Id))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Offerlines, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Offer, Models.Order>()
                // mapping all fields that are shared between the 2 models because they share the same underlying SQL table
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.RelativeBuildingId))
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.RelativeContactId))
                .ForMember(dest => dest.OfferNumber, opt => opt.MapFrom(src => src.Number))
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRateId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.Offerline, Domain.Models.Offerline>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Offer, opt => opt.Ignore())
                .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.Offer.Id))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
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
                .ForMember(dest => dest.RequestNumber, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => ParseDate(src.ExpectedDate)))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => ParseDate(src.RequiredDate)))
                .ForMember(dest => dest.PlanningDate, opt => opt.MapFrom(src => ParseDate(src.PlanningDate)))
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.DepositInvoicesHubs.Select(x => x.DepositInvoice)))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.ExpectedDate != null ? ((DateTime) src.ExpectedDate).ToString("d/MM/yyyy") : null))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate != null ? ((DateTime) src.RequiredDate).ToString("d/MM/yyyy") : null))
                .ForMember(dest => dest.PlanningDate, opt => opt.MapFrom(src => src.PlanningDate != null ? ((DateTime) src.PlanningDate).ToString("d/MM/yyyy") : null))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.Contact != null ? src.Contact.Number : (int?) null))
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.Building != null ? src.Building.Number : (int?) null))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
                .ForMember(dest => dest.Offer, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.DepositInvoicesHubs, opt => opt.Ignore())
                .ForMember(dest => dest.Deposits, opt => opt.Ignore())
                .ForMember(dest => dest.OfferNumber, opt => opt.Ignore()) // Offer number cannot be updated through Order
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.Invoice>()
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.DepositInvoiceHubs.Select(x => x.DepositInvoice)))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.Contact != null ? src.Contact.Number : (int?) null))
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.Building != null ? src.Building.Number : (int?) null))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order != null ? src.Order.Id : (int?) null))
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
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Id : (int?) null))
                .PreserveReferences();

            CreateMap<Models.Deposit, Domain.Models.Deposit>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.Id))
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.Id))
                .ForMember(dest => dest.Payment, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Payment.Id))
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
