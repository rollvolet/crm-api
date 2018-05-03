using System;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Mappers
{
    public class DTOAutoMapperProfileConfiguration : Profile
    {
        public DTOAutoMapperProfileConfiguration()
        {
            // Customer mappings

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Customer, CustomerAttributesDto>().ReverseMap();

            CreateMap<Customer, CustomerRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Customer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"));

            CreateMap<CustomerRequestDto, Customer>()
                .ConstructUsing((src, context) => context.Mapper.Map<Customer>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.NullSubstitute("0"))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Country : null))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Language : null))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.HonorificPrefix : null))
                .ForMember(dest => dest.Telephones, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Telephones : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Tags : null))
                .ForMember(dest => dest.Buildings, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Buildings : null))
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contacts : null))
                .ForMember(dest => dest.Requests, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Requests : null))
                .ForMember(dest => dest.Offers, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Offers : null))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Orders : null))
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.DepositInvoices : null))
                .ForMember(dest => dest.Invoices, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoices : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Customer>()
                .ConstructUsing((src, context) => context.Mapper.Map<Customer>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Customer>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Customer>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());                


            // Contact mappings
            
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));
            
            CreateMap<Contact, ContactAttributesDto>().ReverseMap();

            CreateMap<Contact, ContactRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Contact, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"));

            CreateMap<ContactRequestDto, Contact>()
                .ConstructUsing((src, context) => context.Mapper.Map<Contact>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Country : null))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Language : null))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.HonorificPrefix : null))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForAllOtherMembers(opt => opt.Ignore());   

            CreateMap<OneRelationship, Contact>()
                .ConstructUsing((src, context) => context.Mapper.Map<Contact>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Contact>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Contact>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Contact>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Building mappings

            CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Building, BuildingAttributesDto>().ReverseMap();

            CreateMap<Building, BuildingRelationshipsDto>().ConvertUsing<RelationshipsConverter>();
            
            CreateMap<Building, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"));

            CreateMap<BuildingRequestDto, Building>()
                .ConstructUsing((src, context) => context.Mapper.Map<Building>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Country : null))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Language : null))
                .ForMember(dest => dest.HonorificPrefix, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.HonorificPrefix : null))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForAllOtherMembers(opt => opt.Ignore());  

            CreateMap<OneRelationship, Building>()
                .ConstructUsing((src, context) => context.Mapper.Map<Building>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Building>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Building>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Building>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Country mappings

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Country, CountryDto.AttributesDto>();

            CreateMap<Country, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Country, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"));

            CreateMap<OneRelationship, Country>()
                .ConstructUsing((src, context) => context.Mapper.Map<Country>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Country>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Country>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Country>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Honorific prefix mappings

            CreateMap<HonorificPrefix, HonorificPrefixDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<HonorificPrefix, HonorificPrefixDto.AttributesDto>();

            CreateMap<HonorificPrefix, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<HonorificPrefix, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"));

            CreateMap<OneRelationship, HonorificPrefix>()
                .ConstructUsing((src, context) => context.Mapper.Map<HonorificPrefix>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<HonorificPrefix>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<HonorificPrefix>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, HonorificPrefix>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Language mappings

            CreateMap<Language, LanguageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Language, LanguageDto.AttributesDto>();

            CreateMap<Language, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Language, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"));

            CreateMap<OneRelationship, Language>()
                .ConstructUsing((src, context) => context.Mapper.Map<Language>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Language>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Language>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Language>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Postal code mappings

            CreateMap<PostalCode, PostalCodeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<PostalCode, PostalCodeDto.AttributesDto>();

            CreateMap<PostalCode, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<PostalCode, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"));

            CreateMap<OneRelationship, PostalCode>()
                .ConstructUsing((src, context) => context.Mapper.Map<PostalCode>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<PostalCode>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<PostalCode>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, PostalCode>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());                


            // Telephone mappings

            CreateMap<Telephone, TelephoneDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Telephone, TelephoneAttributesDto>().ReverseMap();

            CreateMap<Telephone, TelephoneRelationshipsDto>().ConvertUsing<RelationshipsConverter>();         

            CreateMap<Telephone, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"));

            CreateMap<TelephoneRequestDto, Telephone>()
                .ConstructUsing((src, context) => context.Mapper.Map<Telephone>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Country : null))
                .ForMember(dest => dest.TelephoneType, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.TelephoneType : null))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Telephone>()
                .ConstructUsing((src, context) => context.Mapper.Map<Telephone>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Telephone>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Telephone>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Telephone>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Telephone type mappings

            CreateMap<TelephoneType, TelephoneTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<TelephoneType, TelephoneTypeDto.AttributesDto>();

            CreateMap<TelephoneType, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<TelephoneType, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"));

            CreateMap<OneRelationship, TelephoneType>()
                .ConstructUsing((src, context) => context.Mapper.Map<TelephoneType>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<TelephoneType>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<TelephoneType>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, TelephoneType>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Tag mappings

            CreateMap<Tag, TagDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("tags"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Tag, TagDto.AttributesDto>();

            CreateMap<Tag, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Tag, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("tags"));

            CreateMap<OneRelationship, Tag>()
                .ConstructUsing((src, context) => context.Mapper.Map<Tag>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Tag>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Tag>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Tag>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Request mappings

            CreateMap<Request, RequestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("requests"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Request, RequestDto.AttributesDto>();

            CreateMap<Request, RequestDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Request, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("requests"));

            CreateMap<OneRelationship, Request>()
                .ConstructUsing((src, context) => context.Mapper.Map<Request>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Request>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Request>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Request>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Way of Entry mappings

            CreateMap<WayOfEntry, WayOfEntryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("way-of-entries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<WayOfEntry, WayOfEntryDto.AttributesDto>();

            CreateMap<WayOfEntry, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<WayOfEntry, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("way-of-entries"));

            CreateMap<OneRelationship, WayOfEntry>()
                .ConstructUsing((src, context) => context.Mapper.Map<WayOfEntry>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<WayOfEntry>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<WayOfEntry>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, WayOfEntry>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());                


            // Visit mappings

            CreateMap<Visit, VisitDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("visits"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Visit, VisitDto.AttributesDto>();

            CreateMap<Visit, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Visit, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("visits"));

            CreateMap<OneRelationship, Visit>()
                .ConstructUsing((src, context) => context.Mapper.Map<Visit>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Visit>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Visit>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Visit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Offer mappings

            CreateMap<Offer, OfferDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("offers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Offer, OfferDto.AttributesDto>();

            CreateMap<Offer, OfferDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Offer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("offers"));

            CreateMap<OneRelationship, Offer>()
                .ConstructUsing((src, context) => context.Mapper.Map<Offer>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Offer>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Offer>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Offer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // VAT Rate mappings

            CreateMap<VatRate, VatRateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("vat-rates"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<VatRate, VatRateDto.AttributesDto>();

            CreateMap<VatRate, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<VatRate, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("vat-rates"));  

            CreateMap<OneRelationship, VatRate>()
                .ConstructUsing((src, context) => context.Mapper.Map<VatRate>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<VatRate>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<VatRate>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, VatRate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Submission types mappings

            CreateMap<SubmissionType, SubmissionTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("submission-types"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<SubmissionType, SubmissionTypeDto.AttributesDto>();

            CreateMap<SubmissionType, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<SubmissionType, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("submission-types"));

            CreateMap<OneRelationship, SubmissionType>()
                .ConstructUsing((src, context) => context.Mapper.Map<SubmissionType>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<SubmissionType>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<SubmissionType>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, SubmissionType>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Order mappings

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("orders"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Order, OrderDto.AttributesDto>();

            CreateMap<Order, OrderDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Order, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("orders"));     

            CreateMap<OneRelationship, Order>()
                .ConstructUsing((src, context) => context.Mapper.Map<Order>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Order>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Order>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());     


            // Invoice mappings

            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("invoices"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Invoice, InvoiceDto.AttributesDto>();

            CreateMap<Invoice, InvoiceDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Invoice, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("invoices"));       

            CreateMap<OneRelationship, Invoice>()
                .ConstructUsing((src, context) => context.Mapper.Map<Invoice>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Invoice>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Invoice>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Deposit Invoice mappings

            CreateMap<DepositInvoice, DepositInvoiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("deposit-invoices"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<DepositInvoice, DepositInvoiceDto.AttributesDto>();

            CreateMap<DepositInvoice, DepositInvoiceDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<DepositInvoice, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("deposit-invoices"));   

            CreateMap<OneRelationship, DepositInvoice>()
                .ConstructUsing((src, context) => context.Mapper.Map<DepositInvoice>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<DepositInvoice>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<DepositInvoice>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, DepositInvoice>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Invoice supplement mappings

            CreateMap<InvoiceSupplement, InvoiceSupplementDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("invoice-supplements"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<InvoiceSupplement, InvoiceSupplementDto.AttributesDto>();

            CreateMap<InvoiceSupplement, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<InvoiceSupplement, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("invoice-supplements"));  

            CreateMap<OneRelationship, InvoiceSupplement>()
                .ConstructUsing((src, context) => context.Mapper.Map<InvoiceSupplement>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<InvoiceSupplement>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<InvoiceSupplement>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, InvoiceSupplement>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());  


            // Deposit mappings

            CreateMap<Deposit, DepositDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("deposits"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Deposit, DepositDto.AttributesDto>();

            CreateMap<Deposit, DepositDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Deposit, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("deposits"));  

            CreateMap<OneRelationship, Deposit>()
                .ConstructUsing((src, context) => context.Mapper.Map<Deposit>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Deposit>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Deposit>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Deposit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore()); 


            // Employee mappings

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("employees"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Employee, EmployeeDto.AttributesDto>();

            CreateMap<Employee, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Employee, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("employees"));  

            CreateMap<OneRelationship, Employee>()
                .ConstructUsing((src, context) => context.Mapper.Map<Employee>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Employee>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Employee>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());



            // Working hour mappings

            CreateMap<WorkingHour, WorkingHourDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("working-hours"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<WorkingHour, WorkingHourDto.AttributesDto>();

            CreateMap<WorkingHour, WorkingHourDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<WorkingHour, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("working-hours"));  

            CreateMap<OneRelationship, WorkingHour>()
                .ConstructUsing((src, context) => context.Mapper.Map<WorkingHour>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<WorkingHour>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<WorkingHour>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, WorkingHour>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Payment mappings

            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("payments"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Payment, PaymentDto.AttributesDto>();

            CreateMap<Payment, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Payment, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("payments"));  

            CreateMap<OneRelationship, Payment>()
                .ConstructUsing((src, context) => context.Mapper.Map<Payment>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Payment>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Payment>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Payment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());                                                                    
        }

    }
}
