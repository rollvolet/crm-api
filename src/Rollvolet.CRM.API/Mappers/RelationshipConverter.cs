using System.Collections.Generic;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Requests;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.DTO.Visits;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Mappers
{
    public class RelationshipsConverter : ITypeConverter<Customer, CustomerRelationshipsDto>,
                                            ITypeConverter<Contact, ContactRelationshipsDto>,
                                            ITypeConverter<Building, BuildingRelationshipsDto>,
                                            ITypeConverter<Telephone, TelephoneRelationshipsDto>,
                                            ITypeConverter<Request, RequestRelationshipsDto>,
                                            ITypeConverter<Visit, VisitRelationshipsDto>,
                                            ITypeConverter<Offer, OfferDto.RelationshipsDto>,
                                            ITypeConverter<Order, OrderDto.RelationshipsDto>,
                                            ITypeConverter<Invoice, InvoiceDto.RelationshipsDto>,
                                            ITypeConverter<DepositInvoice, DepositInvoiceDto.RelationshipsDto>,
                                            ITypeConverter<Deposit, DepositDto.RelationshipsDto>,
                                            ITypeConverter<WorkingHour, WorkingHourDto.RelationshipsDto>,
                                            ITypeConverter<Country, EmptyRelationshipsDto>,
                                            ITypeConverter<Language, EmptyRelationshipsDto>,
                                            ITypeConverter<PostalCode, EmptyRelationshipsDto>,
                                            ITypeConverter<TelephoneType, EmptyRelationshipsDto>,
                                            ITypeConverter<Tag, EmptyRelationshipsDto>,
                                            ITypeConverter<HonorificPrefix, EmptyRelationshipsDto>,
                                            ITypeConverter<WayOfEntry, EmptyRelationshipsDto>,
                                            ITypeConverter<VatRate, EmptyRelationshipsDto>,
                                            ITypeConverter<SubmissionType, EmptyRelationshipsDto>,
                                            ITypeConverter<Payment, EmptyRelationshipsDto>,
                                            ITypeConverter<InvoiceSupplement, EmptyRelationshipsDto>,
                                            ITypeConverter<Employee, EmptyRelationshipsDto>
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
            relationships.Requests = GetManyRelationship<Request>("customers", source.Id, "requests", source.Requests, context);
            relationships.Offers = GetManyRelationship<Offer>("customers", source.Id, "offers", source.Offers, context);
            relationships.Orders = GetManyRelationship<Order>("customers", source.Id, "orders", source.Orders, context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("customers", source.Id, "deposit-invoices", source.DepositInvoices, context);
            relationships.Invoices = GetManyRelationship<Invoice>("customers", source.Id, "invoices", source.Invoices, context);
            relationships.Tags = GetManyRelationship<Tag>("customers", source.Id, "tags", source.Tags, context);
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
            relationships.Visit = GetOneRelationship<Visit>("requests", source.Id, "visit", source.Visit, context);
            relationships.Offer = GetOneRelationship<Offer>("requests", source.Id, "offer", source.Offer, context);
            return relationships;
        }

        OfferDto.RelationshipsDto ITypeConverter<Offer, OfferDto.RelationshipsDto>.Convert(Offer source, OfferDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new OfferDto.RelationshipsDto();
            relationships.Request = GetOneRelationship<Request>("offers", source.Id, "request", source.Request, context);
            relationships.Order = GetOneRelationship<Order>("offers", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("offers", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("offers", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("offers", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("offers", source.Id, "vat-rate", source.VatRate, context);
            relationships.SubmissionType = GetOneRelationship<SubmissionType>("offers", source.Id, "submission-type", source.SubmissionType, context);
            return relationships;
        }

        OrderDto.RelationshipsDto ITypeConverter<Order, OrderDto.RelationshipsDto>.Convert(Order source, OrderDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new OrderDto.RelationshipsDto();
            relationships.Offer = GetOneRelationship<Offer>("orders", source.Id, "offer", source.Offer, context);
            relationships.Invoice = GetOneRelationship<Invoice>("orders", source.Id, "invoice", source.Invoice, context);
            relationships.Customer = GetOneRelationship<Customer>("orders", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("orders", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("orders", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("orders", source.Id, "vat-rate", source.VatRate, context);
            relationships.Deposits = GetManyRelationship<Deposit>("orders", source.Id, "deposits", source.Deposits, context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("orders", source.Id, "deposit-invoices", source.DepositInvoices, context);
            return relationships;
        }

        InvoiceDto.RelationshipsDto ITypeConverter<Invoice, InvoiceDto.RelationshipsDto>.Convert(Invoice source, InvoiceDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new InvoiceDto.RelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("invoices", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("invoices", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("invoices", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("invoices", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("invoices", source.Id, "vat-rate", source.VatRate, context);
            relationships.Supplements = GetManyRelationship<InvoiceSupplement>("invoices", source.Id, "supplements", source.Supplements, context);
            relationships.Deposits = GetManyRelationship<Deposit>("invoices", source.Id, "deposits", source.Deposits, context);
            relationships.DepositInvoices = GetManyRelationship<DepositInvoice>("invoices", source.Id, "deposit-invoices", source.DepositInvoices, context);
            relationships.WorkingHours = GetManyRelationship<WorkingHour>("invoices", source.Id, "working-hours", source.WorkingHours, context);
            return relationships;
        }

        DepositInvoiceDto.RelationshipsDto ITypeConverter<DepositInvoice, DepositInvoiceDto.RelationshipsDto>.Convert(DepositInvoice source, DepositInvoiceDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new DepositInvoiceDto.RelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("deposit-invoices", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("deposit-invoices", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("deposit-invoices", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("deposit-invoices", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("deposit-invoices", source.Id, "vat-rate", source.VatRate, context);
            return relationships;
        }

        DepositDto.RelationshipsDto ITypeConverter<Deposit, DepositDto.RelationshipsDto>.Convert(Deposit source, DepositDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new DepositDto.RelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("deposits", source.Id, "order", source.Order, context);
            relationships.Invoice = GetOneRelationship<Invoice>("deposits", source.Id, "invoice", source.Invoice, context);
            relationships.Customer = GetOneRelationship<Customer>("deposits", source.Id, "customer", source.Customer, context);
            relationships.Payment = GetOneRelationship<Payment>("deposits", source.Id, "payment", source.Payment, context);
            return relationships;
        }

        WorkingHourDto.RelationshipsDto ITypeConverter<WorkingHour, WorkingHourDto.RelationshipsDto>.Convert(WorkingHour source, WorkingHourDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new WorkingHourDto.RelationshipsDto();
            relationships.Employee = GetOneRelationship<Employee>("working-hours", source.Id, "employee", source.Employee, context);
            relationships.Invoice = GetOneRelationship<Invoice>("working-hours", source.Id, "invoice", source.Invoice, context);
            return relationships;
        }

        VisitRelationshipsDto ITypeConverter<Visit, VisitRelationshipsDto>.Convert(Visit source, VisitRelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new VisitRelationshipsDto();
            relationships.Customer = GetOneRelationship<Customer>("visits", source.Id, "customer", source.Customer, context);
            relationships.Request = GetOneRelationship<Request>("visits", source.Id, "request", source.Request, context);
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

        EmptyRelationshipsDto ITypeConverter<VatRate, EmptyRelationshipsDto>.Convert(VatRate source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<SubmissionType, EmptyRelationshipsDto>.Convert(SubmissionType source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<Payment, EmptyRelationshipsDto>.Convert(Payment source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<InvoiceSupplement, EmptyRelationshipsDto>.Convert(InvoiceSupplement source, EmptyRelationshipsDto destination, ResolutionContext context)
        {
            return new EmptyRelationshipsDto();
        }

        EmptyRelationshipsDto ITypeConverter<Employee, EmptyRelationshipsDto>.Convert(Employee source, EmptyRelationshipsDto destination, ResolutionContext context)
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