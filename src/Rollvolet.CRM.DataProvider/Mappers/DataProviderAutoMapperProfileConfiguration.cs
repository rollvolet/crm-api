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
                .ForMember(dest => dest.CustomerTags, opt => opt.Ignore())
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
                .ForMember(dest => dest.OriginId, opt => opt.MapFrom(src => src.Origin.Id))
                .ForMember(dest => dest.Origin, opt => opt.Ignore())
                .ForMember(dest => dest.Offer, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.WayOfEntry, Domain.Models.WayOfEntry>()
                .PreserveReferences()
                .ReverseMap()
                .PreserveReferences();

            CreateMap<Models.Intervention, Domain.Models.Intervention>()
                .ForMember(dest => dest.Technicians, opt => opt.Ignore())
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
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.InterventionTechnicians, opt => opt.MapFrom(src => src.Technicians))
                .ForMember(dest => dest.Origin, opt => opt.Ignore())
                .ForMember(dest => dest.OriginId, opt => opt.MapFrom(src => src.Origin.Id))
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.FollowUpRequest, opt => opt.Ignore())
                .ForMember(dest => dest.PlanningEvent, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    foreach(var joinEntry in dest.InterventionTechnicians)
                    {
                        joinEntry.InterventionId = dest.Id;
                    }
                })
                .PreserveReferences();

            CreateMap<Models.PlanningEvent, Domain.Models.PlanningEvent>()
                .ForMember(dest => dest.Period, opt => opt.Ignore())
                .ForMember(dest => dest.FromHour, opt => opt.Ignore())
                .ForMember(dest => dest.UntilHour, opt => opt.Ignore())
                .ForMember(dest => dest.IsNotAvailableInCalendar, opt => opt.Ignore())
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.MsObjectId, opt => opt.MapFrom(src => src.MsObjectId))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                .ForMember(dest => dest.Intervention, opt => opt.Ignore())
                .ForMember(dest => dest.InterventionId, opt => opt.MapFrom(src => src.Intervention.Id))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.Id))
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
                .ForMember(dest => dest.Technicians, opt => opt.Ignore())
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
                .ForMember(dest => dest.Interventions, opt => opt.Ignore())
                .ForMember(dest => dest.OrderTechnicians, opt => opt.MapFrom(src => src.Technicians))
                .ForMember(dest => dest.OfferNumber, opt => opt.Ignore()) // Offer number cannot be updated through Order
                .AfterMap((src, dest) => {
                    foreach(var joinEntry in dest.OrderTechnicians)
                    {
                        joinEntry.OrderId = dest.Id;
                    }
                })
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
                .ForMember(dest => dest.Intervention, opt => opt.Ignore())
                .ForMember(dest => dest.InterventionId, opt => opt.MapFrom(src => src.Intervention != null ? src.Intervention.Id : (int?) null))
                .ForMember(dest => dest.Deposits, opt => opt.Ignore())
                .ForMember(dest => dest.DepositInvoiceHubs, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.Invoice, Domain.Models.DepositInvoice>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.MainInvoiceHub != null ? src.MainInvoiceHub.Order : null))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.HasProductionTicket, opt => opt.MapFrom(src => false))
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
                .ForMember(dest => dest.DepositInvoiceHubs, opt => opt.Ignore())
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
                .ForMember(dest => dest.Interventions, opt => opt.MapFrom(src => src.InterventionTechnicians))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.OrderTechnicians))
                .PreserveReferences()
                .ReverseMap()
                .ForMember(dest => dest.InterventionTechnicians, opt => opt.Ignore())
                .ForMember(dest => dest.OrderTechnicians, opt => opt.Ignore())
                .PreserveReferences();

            CreateMap<Models.InterventionTechnician, Domain.Models.Intervention>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InterventionId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.InterventionTechnician, Domain.Models.Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Domain.Models.Employee, Models.InterventionTechnician>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.OrderTechnician, Domain.Models.Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Models.OrderTechnician, Domain.Models.Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Domain.Models.Employee, Models.OrderTechnician>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());

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
