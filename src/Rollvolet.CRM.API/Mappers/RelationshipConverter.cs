using System.Collections.Generic;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.DepositInvoices;
using Rollvolet.CRM.APIContracts.DTO.Deposits;
using Rollvolet.CRM.APIContracts.DTO.Invoices;
using Rollvolet.CRM.APIContracts.DTO.InvoiceSupplements;
using Rollvolet.CRM.APIContracts.DTO.Offerlines;
using Rollvolet.CRM.APIContracts.DTO.Offers;
using Rollvolet.CRM.APIContracts.DTO.Orders;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.DTO.CalendarEvents;
using Rollvolet.CRM.APIContracts.DTO.WorkingHours;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.APIContracts.DTO.Invoicelines;
using Rollvolet.CRM.APIContracts.DTO.Interventions;
using Rollvolet.CRM.APIContracts.DTO.PlanningEvents;
using Rollvolet.CRM.Business.Models;

namespace Rollvolet.CRM.API.Mappers
{
    public class RelationshipsConverter : ITypeConverter<Customer, CustomerRelationshipsDto>,
                                            ITypeConverter<Contact, ContactRelationshipsDto>,
                                            ITypeConverter<Building, BuildingRelationshipsDto>,
                                            ITypeConverter<Telephone, TelephoneRelationshipsDto>,
                                            ITypeConverter<Request, RequestRelationshipsDto>,
                                            ITypeConverter<CalendarEvent, CalendarEventRelationshipsDto>,
                                            ITypeConverter<Intervention, InterventionRelationshipsDto>,
                                            ITypeConverter<Offer, OfferRelationshipsDto>,
                                            ITypeConverter<Offerline, OfferlineRelationshipsDto>,
                                            ITypeConverter<Order, OrderRelationshipsDto>,
                                            ITypeConverter<Invoiceline, InvoicelineRelationshipsDto>,
                                            ITypeConverter<Invoice, InvoiceRelationshipsDto>,
                                            ITypeConverter<DepositInvoice, DepositInvoiceRelationshipsDto>,
                                            ITypeConverter<Deposit, DepositRelationshipsDto>,
                                            ITypeConverter<WorkingHour, WorkingHourRelationshipsDto>,
                                            ITypeConverter<InvoiceSupplement, InvoiceSupplementRelationshipsDto>,
                                            ITypeConverter<PlanningEvent, PlanningEventRelationshipsDto>,
                                            ITypeConverter<Employee, EmployeeDto.RelationshipsDto>,
                                            ITypeConverter<Country, EmptyRelationshipsDto>,
                                            ITypeConverter<Language, EmptyRelationshipsDto>,
                                            ITypeConverter<PostalCode, EmptyRelationshipsDto>,
                                            ITypeConverter<TelephoneType, EmptyRelationshipsDto>,
                                            ITypeConverter<Tag, EmptyRelationshipsDto>,
                                            ITypeConverter<HonorificPrefix, EmptyRelationshipsDto>,
                                            ITypeConverter<WayOfEntry, EmptyRelationshipsDto>,
                                            ITypeConverter<ProductUnit, EmptyRelationshipsDto>,
                                            ITypeConverter<VatRate, EmptyRelationshipsDto>,
                                            ITypeConverter<Payment, EmptyRelationshipsDto>,
                                            ITypeConverter<AccountancyExport, EmptyRelationshipsDto>,
                                            ITypeConverter<MonthlySalesEntry, EmptyRelationshipsDto>,
                                            ITypeConverter<OutstandingJob, EmptyRelationshipsDto>,
                                            ITypeConverter<OutstandingJobReport, EmptyRelationshipsDto>,
                                            ITypeConverter<ErrorNotification, EmptyRelationshipsDto>
    {
        CustomerRelationshipsDto ITypeConverter<Customer, CustomerRelationshipsDto>.Convert(Customer source, CustomerRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new CustomerRelationshipsDto();
            relationships.Contacts = GetManyRelationship<Contact>("customers", source.Id, "contacts", source.Contacts, context);
            relationships.Buildings = GetManyRelationship<Building>("customers", source.Id, "buildings", source.Buildings, context);
            relationships.Country = GetOneRelationship<Country>("customers", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("customers", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("customers", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Telephones = GetManyRelationship<Telephone>("customers", source.Id, "telephones", source.Telephones, context);
            relationships.Tags = GetManyRelationship<Tag>("customers", source.Id, "tags", source.Tags, context);
            relationships.Requests = GetManyRelationship<Request>("customers", source.Id, "requests", new List<Request>(), context);
            relationships.Interventions = GetManyRelationship<Intervention>("customers", source.Id, "interventions", new List<Intervention>(), context);
            relationships.Offers = GetManyRelationship<Offer>("customers", source.Id, "offers", new List<Offer>(), context);
            relationships.Orders = GetManyRelationship<Order>("customers", source.Id, "orders", new List<Order>(), context);
            relationships.Invoices = GetManyRelationship<Invoice>("customers", source.Id, "invoices", new List<Invoice>(), context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("customers", source.Id, "deposit-invoices", new List<DepositInvoice>(), context);
            return relationships;
        }

        ContactRelationshipsDto ITypeConverter<Contact, ContactRelationshipsDto>.Convert(Contact source, ContactRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new ContactRelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("contacts", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("contacts", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("contacts", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Customer = GetOneRelationship<Customer>("contacts", source.Id, "customer", source.Customer, context);
            relationships.Telephones = GetManyRelationship<Telephone>("contacts", source.Id, "telephones", source.Telephones, context);
            return relationships;
        }

        BuildingRelationshipsDto ITypeConverter<Building, BuildingRelationshipsDto>.Convert(Building source, BuildingRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new BuildingRelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("buildings", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("buildings", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("buildings", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Customer = GetOneRelationship<Customer>("buildings", source.Id, "customer", source.Customer, context);
            relationships.Telephones = GetManyRelationship<Telephone>("buildings", source.Id, "telephones", source.Telephones, context);
            return relationships;
        }

        TelephoneRelationshipsDto ITypeConverter<Telephone, TelephoneRelationshipsDto>.Convert(Telephone source, TelephoneRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new TelephoneRelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("telephones", source.Id, "country", source.Country, context);
            relationships.TelephoneType = GetOneRelationship<TelephoneType>("telephones", source.Id, "telephone-type", source.TelephoneType, context);
            relationships.Customer = GetOneRelationship<Customer>("telephones", source.Id, "customer", source.Customer, context);
            relationships.Contact = GetOneRelationship<Contact>("telephones", source.Id, "contact", source.Contact, context);
            relationships.Building = GetOneRelationship<Building>("telephones", source.Id, "building", source.Building, context);
            return relationships;
        }

        RequestRelationshipsDto ITypeConverter<Request, RequestRelationshipsDto>.Convert(Request source, RequestRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new RequestRelationshipsDto();
            relationships.Customer = GetOneRelationship<Customer>("requests", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("requests", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("requests", source.Id, "contact", source.Contact, context);
            relationships.WayOfEntry = GetOneRelationship<WayOfEntry>("requests", source.Id, "way-of-entry", source.WayOfEntry, context);
            relationships.CalendarEvent = GetOneRelationship<CalendarEvent>("requests", source.Id, "calendar-event", source.CalendarEvent, context);
            relationships.Offer = GetOneRelationship<Offer>("requests", source.Id, "offer", source.Offer, context);
            relationships.Origin = GetOneRelationship<Intervention>("requests", source.Id, "origin", source.Origin, context);
            return relationships;
        }

        InterventionRelationshipsDto ITypeConverter<Intervention, InterventionRelationshipsDto>.Convert(Intervention source, InterventionRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new InterventionRelationshipsDto();
            relationships.Customer = GetOneRelationship<Customer>("interventions", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("interventions", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("interventions", source.Id, "contact", source.Contact, context);
            relationships.WayOfEntry = GetOneRelationship<WayOfEntry>("interventions", source.Id, "way-of-entry", source.WayOfEntry, context);
            relationships.FollowUpRequest = GetOneRelationship<Request>("interventions", source.Id, "follow-up-request", source.FollowUpRequest, context);
            relationships.Origin = GetOneRelationship<Order>("interventions", source.Id, "origin", source.Origin, context);
            relationships.PlanningEvent = GetOneRelationship<PlanningEvent>("interventions", source.Id, "planning-event", source.PlanningEvent, context);
            relationships.Invoice = GetOneRelationship<Invoice>("interventions", source.Id, "invoice", source.Invoice, context);
            relationships.Employee = GetOneRelationship<Employee>("interventions", source.Id, "employee", source.Employee, context);
            relationships.Technicians = GetManyRelationship<Employee>("interventions", source.Id, "technicians", source.Technicians, context);
            return relationships;
        }

        OfferRelationshipsDto ITypeConverter<Offer, OfferRelationshipsDto>.Convert(Offer source, OfferRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new OfferRelationshipsDto();
            relationships.Request = GetOneRelationship<Request>("offers", source.Id, "request", source.Request, context);
            relationships.Order = GetOneRelationship<Order>("offers", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("offers", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("offers", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("offers", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("offers", source.Id, "vat-rate", source.VatRate, context);
            relationships.Offerlines = GetManyRelationship<Offerline>("offers", source.Id, "offerlines", source.Offerlines, context);
            return relationships;
        }

        OfferlineRelationshipsDto ITypeConverter<Offerline, OfferlineRelationshipsDto>.Convert(Offerline source, OfferlineRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new OfferlineRelationshipsDto();
            relationships.Offer = GetOneRelationship<Offer>("offerlines", source.Id, "offer", source.Offer, context);
            relationships.VatRate = GetOneRelationship<VatRate>("offerlines", source.Id, "vat-rate", source.VatRate, context);
            return relationships;
        }

        OrderRelationshipsDto ITypeConverter<Order, OrderRelationshipsDto>.Convert(Order source, OrderRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new OrderRelationshipsDto();
            relationships.Offer = GetOneRelationship<Offer>("orders", source.Id, "offer", source.Offer, context);
            relationships.Invoice = GetOneRelationship<Invoice>("orders", source.Id, "invoice", source.Invoice, context);
            relationships.Customer = GetOneRelationship<Customer>("orders", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("orders", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("orders", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("orders", source.Id, "vat-rate", source.VatRate, context);
            relationships.Invoicelines = GetManyRelationship<Invoiceline>("orders", source.Id, "invoicelines", source.Invoicelines, context);
            relationships.Deposits = GetManyRelationship<Deposit>("orders", source.Id, "deposits", source.Deposits, context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("orders", source.Id, "deposit-invoices", source.DepositInvoices, context);
            relationships.Interventions = GetManyRelationship<Intervention>("orders", source.Id, "interventions", source.Interventions, context);
            relationships.Technicians = GetManyRelationship<Employee>("orders", source.Id, "technicians", source.Technicians, context);
            return relationships;
        }

        InvoicelineRelationshipsDto ITypeConverter<Invoiceline, InvoicelineRelationshipsDto>.Convert(Invoiceline source, InvoicelineRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new InvoicelineRelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("invoicelines", source.Id, "order", source.Order, context);
            relationships.Invoice = GetOneRelationship<Invoice>("invoicelines", source.Id, "invoice", source.Invoice, context);
            relationships.VatRate = GetOneRelationship<VatRate>("invoicelines", source.Id, "vat-rate", source.VatRate, context);
            return relationships;
        }

        InvoiceRelationshipsDto ITypeConverter<Invoice, InvoiceRelationshipsDto>.Convert(Invoice source, InvoiceRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new InvoiceRelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("invoices", source.Id, "order", source.Order, context);
            relationships.Intervention = GetOneRelationship<Intervention>("invoices", source.Id, "intervention", source.Intervention, context);
            relationships.Customer = GetOneRelationship<Customer>("invoices", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("invoices", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("invoices", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("invoices", source.Id, "vat-rate", source.VatRate, context);
            relationships.Supplements = GetManyRelationship<InvoiceSupplement>("invoices", source.Id, "supplements", source.Supplements, context);
            relationships.Invoicelines = GetManyRelationship<Invoiceline>("invoices", source.Id, "invoicelines", source.Invoicelines, context);
            relationships.Deposits = GetManyRelationship<Deposit>("invoices", source.Id, "deposits", source.Deposits, context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("invoices", source.Id, "deposit-invoices", source.DepositInvoices, context);
            relationships.WorkingHours = GetManyRelationship<WorkingHour>("invoices", source.Id, "working-hours", source.WorkingHours, context);
            return relationships;
        }

        DepositInvoiceRelationshipsDto ITypeConverter<DepositInvoice, DepositInvoiceRelationshipsDto>.Convert(DepositInvoice source, DepositInvoiceRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new DepositInvoiceRelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("deposit-invoices", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("deposit-invoices", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("deposit-invoices", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("deposit-invoices", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("deposit-invoices", source.Id, "vat-rate", source.VatRate, context);
            return relationships;
        }

        DepositRelationshipsDto ITypeConverter<Deposit, DepositRelationshipsDto>.Convert(Deposit source, DepositRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new DepositRelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("deposits", source.Id, "order", source.Order, context);
            relationships.Invoice = GetOneRelationship<Invoice>("deposits", source.Id, "invoice", source.Invoice, context);
            relationships.Customer = GetOneRelationship<Customer>("deposits", source.Id, "customer", source.Customer, context);
            relationships.Payment = GetOneRelationship<Payment>("deposits", source.Id, "payment", source.Payment, context);
            return relationships;
        }

        WorkingHourRelationshipsDto ITypeConverter<WorkingHour, WorkingHourRelationshipsDto>.Convert(WorkingHour source, WorkingHourRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new WorkingHourRelationshipsDto();
            relationships.Employee = GetOneRelationship<Employee>("working-hours", source.Id, "employee", source.Employee, context);
            relationships.Invoice = GetOneRelationship<Invoice>("working-hours", source.Id, "invoice", source.Invoice, context);
            return relationships;
        }

        PlanningEventRelationshipsDto ITypeConverter<PlanningEvent, PlanningEventRelationshipsDto>.Convert(PlanningEvent source, PlanningEventRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new PlanningEventRelationshipsDto();
            relationships.Intervention = GetOneRelationship<Intervention>("planning-events", source.Id, "intervention", source.Intervention, context);
            relationships.Order = GetOneRelationship<Order>("planning-events", source.Id, "order", source.Order, context);
            return relationships;
        }

        CalendarEventRelationshipsDto ITypeConverter<CalendarEvent, CalendarEventRelationshipsDto>.Convert(CalendarEvent source, CalendarEventRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new CalendarEventRelationshipsDto();
            relationships.Customer = GetOneRelationship<Customer>("calendar-events", source.Id, "customer", source.Customer, context);
            relationships.Request = GetOneRelationship<Request>("calendar-events", source.Id, "request", source.Request, context);
            return relationships;
        }

        InvoiceSupplementRelationshipsDto ITypeConverter<InvoiceSupplement, InvoiceSupplementRelationshipsDto>.Convert(InvoiceSupplement source, InvoiceSupplementRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new InvoiceSupplementRelationshipsDto();
            relationships.Invoice = GetOneRelationship<Invoice>("invoice-supplements", source.Id, "invoice", source.Invoice, context);
            relationships.Unit = GetOneRelationship<ProductUnit>("invoice-supplements", source.Id, "unit", source.Unit, context);
            return relationships;
        }

        EmployeeDto.RelationshipsDto ITypeConverter<Employee, EmployeeDto.RelationshipsDto>.Convert(Employee source, EmployeeDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new EmployeeDto.RelationshipsDto();
            relationships.WorkingHours = GetManyRelationship<WorkingHour>("employees", source.Id, "working-hours", source.WorkingHours, context);
            return relationships;
        }

        EmptyRelationshipsDto ITypeConverter<Country, EmptyRelationshipsDto>.Convert(Country source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<Language, EmptyRelationshipsDto>.Convert(Language source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<PostalCode, EmptyRelationshipsDto>.Convert(PostalCode source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<TelephoneType, EmptyRelationshipsDto>.Convert(TelephoneType source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<HonorificPrefix, EmptyRelationshipsDto>.Convert(HonorificPrefix source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<Tag, EmptyRelationshipsDto>.Convert(Tag source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<WayOfEntry, EmptyRelationshipsDto>.Convert(WayOfEntry source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<ProductUnit, EmptyRelationshipsDto>.Convert(ProductUnit source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<VatRate, EmptyRelationshipsDto>.Convert(VatRate source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<Payment, EmptyRelationshipsDto>.Convert(Payment source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<AccountancyExport, EmptyRelationshipsDto>.Convert(AccountancyExport source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<ErrorNotification, EmptyRelationshipsDto>.Convert(ErrorNotification source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<MonthlySalesEntry, EmptyRelationshipsDto>.Convert(MonthlySalesEntry source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<OutstandingJob, EmptyRelationshipsDto>.Convert(OutstandingJob source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<OutstandingJobReport, EmptyRelationshipsDto>.Convert(OutstandingJobReport source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        private IRelationship GetManyRelationship<T>(string resourceName, int id, string relatedResourceName, IEnumerable<T> relatedResources, ResolutionContext context)
        {
            return GetManyRelationship<T>(resourceName, id.ToString(), relatedResourceName, relatedResources, context);
        }

        private IRelationship GetManyRelationship<T>(string resourceName, string id, string relatedResourceName, IEnumerable<T> relatedResources, ResolutionContext context)
        {
            if (context.Items.Keys.Contains("include") && ((IncludeQuery) context.Items["include"]).Contains(relatedResourceName))
            {
                return new ManyRelationship() {
                    Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                    Data = context.Mapper.Map<IEnumerable<RelatedResource>>(relatedResources)
                };
            }
            else
            {
                return new Relationship() { Links = GetRelationshipLinks(resourceName, id, relatedResourceName) };
            }
        }

        private IRelationship GetOneRelationship<T>(string resourceName, int id, string relatedResourceName, T relatedResource, ResolutionContext context)
        {
            return GetOneRelationship<T>(resourceName, id.ToString(), relatedResourceName, relatedResource, context);
        }

        private IRelationship GetOneRelationship<T>(string resourceName, string id, string relatedResourceName, T relatedResource, ResolutionContext context)
        {
            if (context.Items.Keys.Contains("include") && ((IncludeQuery) context.Items["include"]).Contains(relatedResourceName))
            {
                return new OneRelationship() {
                    Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                    Data = context.Mapper.Map<RelatedResource>(relatedResource)
                };
            }
            else
            {
                return new Relationship() { Links = GetRelationshipLinks(resourceName, id, relatedResourceName) };
            }
        }

        private Links GetRelationshipLinks(string resourceName, string id, string relationName) {
            return new Links {
                // TODO api prefix must be configurable
                Self = $"/api/{resourceName}/{id}/links/{relationName}",
                Related = $"/api/{resourceName}/{id}/{relationName}"
            };
        }
  }
}