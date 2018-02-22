using System.Collections.Generic;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Mappers
{
    public class RelationshipsConverter : ITypeConverter<Customer, CustomerDto.RelationshipsDto>,
                                            ITypeConverter<Contact, ContactDto.RelationshipsDto>,
                                            ITypeConverter<Building, BuildingDto.RelationshipsDto>,
                                            ITypeConverter<Telephone, TelephoneDto.RelationshipsDto>,
                                            ITypeConverter<Request, RequestDto.RelationshipsDto>,
                                            ITypeConverter<Offer, OfferDto.RelationshipsDto>,
                                            ITypeConverter<Order, OrderDto.RelationshipsDto>,
                                            ITypeConverter<Deposit, DepositDto.RelationshipsDto>,
                                            ITypeConverter<Country, EmptyRelationshipsDto>,
                                            ITypeConverter<Language, EmptyRelationshipsDto>,
                                            ITypeConverter<PostalCode, EmptyRelationshipsDto>,
                                            ITypeConverter<TelephoneType, EmptyRelationshipsDto>,
                                            ITypeConverter<Tag, EmptyRelationshipsDto>,
                                            ITypeConverter<HonorificPrefix, EmptyRelationshipsDto>,
                                            ITypeConverter<WayOfEntry, EmptyRelationshipsDto>,
                                            ITypeConverter<Visit, EmptyRelationshipsDto>,
                                            ITypeConverter<VatRate, EmptyRelationshipsDto>,
                                            ITypeConverter<SubmissionType, EmptyRelationshipsDto>,
                                            ITypeConverter<Payment, EmptyRelationshipsDto>
    {
        CustomerDto.RelationshipsDto ITypeConverter<Customer, CustomerDto.RelationshipsDto>.Convert(Customer source, CustomerDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new CustomerDto.RelationshipsDto();
            relationships.Contacts = GetManyRelationship<Contact>("customers", source.Id, "contacts", source.Contacts, context);
            relationships.Buildings = GetManyRelationship<Building>("customers", source.Id, "buildings", source.Buildings, context);
            relationships.Country = GetOneRelationship<Country>("customers", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("customers", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("customers", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Telephones = GetManyRelationship<Telephone>("customers", source.Id, "telephones", source.Telephones, context);
            relationships.Requests = GetManyRelationship<Request>("customers", source.Id, "requests", source.Requests, context);
            relationships.Offers = GetManyRelationship<Offer>("customers", source.Id, "offers", source.Offers, context);
            relationships.Orders = GetManyRelationship<Order>("customers", source.Id, "orders", source.Orders, context);
            relationships.Tags = GetManyRelationship<Tag>("customers", source.Id, "tags", source.Tags, context);
            return relationships;
        }

        ContactDto.RelationshipsDto ITypeConverter<Contact, ContactDto.RelationshipsDto>.Convert(Contact source, ContactDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new ContactDto.RelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("contacts", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("contacts", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("contacts", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Telephones = GetManyRelationship<Telephone>("contacts", source.Id, "telephones", source.Telephones, context);
            return relationships;
        }

        BuildingDto.RelationshipsDto ITypeConverter<Building, BuildingDto.RelationshipsDto>.Convert(Building source, BuildingDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new BuildingDto.RelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("buildings", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("buildings", source.Id, "language", source.Language, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("buildings", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Telephones = GetManyRelationship<Telephone>("buildings", source.Id, "telephones", source.Telephones, context);
            return relationships;
        }

        TelephoneDto.RelationshipsDto ITypeConverter<Telephone, TelephoneDto.RelationshipsDto>.Convert(Telephone source, TelephoneDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new TelephoneDto.RelationshipsDto();
            relationships.Country = GetOneRelationship<Country>("telephones", source.Id, "country", source.Country, context);
            relationships.TelephoneType = GetOneRelationship<TelephoneType>("telephones", source.Id, "telephone-type", source.TelephoneType, context);
            return relationships;
        }

        RequestDto.RelationshipsDto ITypeConverter<Request, RequestDto.RelationshipsDto>.Convert(Request source, RequestDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new RequestDto.RelationshipsDto();
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
            relationships.Customer = GetOneRelationship<Customer>("orders", source.Id, "customer", source.Customer, context);
            relationships.Building = GetOneRelationship<Building>("orders", source.Id, "building", source.Building, context);
            relationships.Contact = GetOneRelationship<Contact>("orders", source.Id, "contact", source.Contact, context);
            relationships.VatRate = GetOneRelationship<VatRate>("orders", source.Id, "vat-rate", source.VatRate, context);
            relationships.Deposits = GetManyRelationship<Deposit>("orders", source.Id, "deposits", source.Deposits, context);
            return relationships;
        } 

        DepositDto.RelationshipsDto ITypeConverter<Deposit, DepositDto.RelationshipsDto>.Convert(Deposit source, DepositDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new DepositDto.RelationshipsDto();
            relationships.Order = GetOneRelationship<Order>("deposits", source.Id, "order", source.Order, context);
            relationships.Customer = GetOneRelationship<Customer>("deposits", source.Id, "customer", source.Customer, context);
            relationships.Payment = GetOneRelationship<Payment>("deposits", source.Id, "payment", source.Payment, context);
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

        EmptyRelationshipsDto ITypeConverter<Visit, EmptyRelationshipsDto>.Convert(Visit source, EmptyRelationshipsDto destination, ResolutionContext context)
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
                Self = $"/{resourceName}/{id}/links/{relationName}",
                Related = $"/{resourceName}/{id}/{relationName}"
            };
        }
  }
}