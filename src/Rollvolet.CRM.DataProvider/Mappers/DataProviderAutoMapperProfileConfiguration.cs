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
                .ForMember(dest => dest.BookingDate, opt => opt.Ignore())
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
                .ForMember(dest => dest.Requests, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore())
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
                .ForMember(dest => dest.Requests, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore())
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
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Visit != null ? src.Visit.Comment : null))
                .ForMember(dest => dest.Visitor, opt => opt.MapFrom(src => src.Visit != null ? src.Visit.Visitor : null))
                .ForMember(dest => dest.OfferExpected, opt => opt.MapFrom(src => src.Visit != null ? src.Visit.OfferExpected : false))
                .ForMember(dest => dest.CalendarEvent, opt => opt.MapFrom(src => src.Visit))
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

            CreateMap<Models.ProductUnit, Domain.Models.ProductUnit>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Visit, Domain.Models.Request>()
                // only merge the fields from visit that needs to be public in the request object
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Visitor, opt => opt.MapFrom(src => src.Visitor))
                .ForMember(dest => dest.OfferExpected, opt => opt.MapFrom(src => src.OfferExpected))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.Visit, Domain.Models.CalendarEvent>()
                .ConvertUsing(new VisitTypeConverter());

            CreateMap<Domain.Models.CalendarEvent, Models.Visit>()
                // don't map the Id, Request and Customer properties.
                // They are managed by the RequestDataProvider which creates Visit records without using the mapper
                // Also don't map the fields that are set by the request domain model (visitor, offerExpected, comment)
                .ForMember(dest => dest.VisitDate, opt => opt.MapFrom(src => src.VisitDate))
                .ForMember(dest => dest.CalendarId, opt => opt.MapFrom(src => src.CalendarId))
                .ForMember(dest => dest.MsObjectId, opt => opt.MapFrom(src => src.MsObjectId))
                .ForMember(dest => dest.CalendarSubject, opt => opt.MapFrom(src => src.CalendarSubject))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.Offerline, Domain.Models.Offerline>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Offer, opt => opt.Ignore())
                .ForMember(dest => dest.OfferId, opt => opt.MapFrom(src => src.Offer.Id))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
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
                .ForMember(dest => dest.Deposits, opt => opt.Ignore())
                .ForMember(dest => dest.Supplements, opt => opt.Ignore())
                .ForMember(dest => dest.DepositInvoiceHubs, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.DepositInvoice>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.MainInvoiceHub != null ? src.MainInvoiceHub.Order : null))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.HasProductionTicket, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.IsCreditNote, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.Contact, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeContactId, opt => opt.MapFrom(src => src.Contact != null ? src.Contact.Number : (int?) null))
                .ForMember(dest => dest.Building, opt => opt.Ignore())
                .ForMember(dest => dest.RelativeBuildingId, opt => opt.MapFrom(src => src.Building != null ? src.Building.Number : (int?) null))
                .ForMember(dest => dest.VatRate, opt => opt.Ignore())
                .ForMember(dest => dest.VatRateId, opt => opt.MapFrom(src => src.VatRate != null ? src.VatRate.Id : null))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore()) // order id is not set on invoice record, but on deposit invoice hub record
                .ForMember(dest => dest.Deposits, opt => opt.Ignore())
                .ForMember(dest => dest.Supplements, opt => opt.Ignore())
                .ForMember(dest => dest.DepositInvoiceHubs, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.InvoiceSupplement, Domain.Models.InvoiceSupplement>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Id : (int?) null))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Id : (int?) null))
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
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.Id))
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName))
                .PreserveReferences();

            CreateMap<Models.Payment, Domain.Models.Payment>()
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "BETALING"))
                .PreserveReferences();

            CreateMap<Models.AccountancyExport, Domain.Models.AccountancyExport>()
                .PreserveReferences()
                .ReverseMap()
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
                if (DateTime.TryParseExact(dateString, "d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else if (DateTime.TryParseExact(dateString, "d/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    return dateTime;
                else
                    return null;
            }
        }
    }
}
