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
using Rollvolet.CRM.APIContracts.DTO.AccountancyExports;
using Rollvolet.CRM.APIContracts.DTO.ErrorNotifications;
using Rollvolet.CRM.APIContracts.DTO.ProductUnits;
using Rollvolet.CRM.APIContracts.DTO.Invoicelines;
using Rollvolet.CRM.APIContracts.DTO.Interventions;

namespace Rollvolet.CRM.API.Mappers
{
    public class DTOAutoMapperProfileConfiguration : Profile
    {
        public DTOAutoMapperProfileConfiguration()
        {
            // Case mappings

            CreateMap<Case, CaseDto>()
                .ReverseMap();


            // Customer mappings

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "customers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Customer, CustomerAttributesDto>()
                .ReverseMap()
                .ForMember(dest => dest.VatNumber, opt => opt.MapFrom(src => Customer.SerializeVatNumber(src.VatNumber)));

            CreateMap<Customer, CustomerRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Customer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "customers"));

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
                .ForMember(dest => dest.Interventions, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Interventions : null))
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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Contact, ContactAttributesDto>().ReverseMap();

            CreateMap<Contact, ContactRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Contact, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "contacts"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Building, BuildingAttributesDto>().ReverseMap();

            CreateMap<Building, BuildingRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Building, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "buildings"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "countries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Country, CountryDto.AttributesDto>();

            CreateMap<Country, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Country, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "countries"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "honorific-prefixes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<HonorificPrefix, HonorificPrefixDto.AttributesDto>();

            CreateMap<HonorificPrefix, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<HonorificPrefix, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "honorific-prefixes"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "languages"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Language, LanguageDto.AttributesDto>();

            CreateMap<Language, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Language, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "languages"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "postal-codes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<PostalCode, PostalCodeDto.AttributesDto>();

            CreateMap<PostalCode, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<PostalCode, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "postal-codes"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "telephones"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Telephone, TelephoneAttributesDto>()
                .ReverseMap()
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Telephone.SerializeNumber(src.Number)))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => Telephone.SerializeArea(src.Area)));

            CreateMap<Telephone, TelephoneRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Telephone, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "telephones"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "telephone-types"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<TelephoneType, TelephoneTypeDto.AttributesDto>();

            CreateMap<TelephoneType, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<TelephoneType, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "telephone-types"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "tags"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Tag, TagDto.AttributesDto>();

            CreateMap<Tag, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Tag, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "tags"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "requests"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Request, RequestAttributesDto>().ReverseMap();

            CreateMap<Request, RequestRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Request, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "requests"));

            CreateMap<RequestRequestDto, Request>()
                .ConstructUsing((src, context) => context.Mapper.Map<Request>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.WayOfEntry, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.WayOfEntry : null))
                .ForMember(dest => dest.CalendarEvent, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.CalendarEvent : null))
                .ForMember(dest => dest.Offer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Offer : null))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Origin : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Request>()
                .ConstructUsing((src, context) => context.Mapper.Map<Request>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Request>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Request>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Request>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Intervention mappings

            CreateMap<Intervention, InterventionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "interventions"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Intervention, InterventionAttributesDto>().ReverseMap();

            CreateMap<Intervention, InterventionRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Intervention, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "interventions"));

            CreateMap<InterventionRequestDto, Intervention>()
                .ConstructUsing((src, context) => context.Mapper.Map<Intervention>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.WayOfEntry, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.WayOfEntry : null))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Origin : null))
                .ForMember(dest => dest.FollowUpRequest, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.FollowUpRequest : null))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.Technicians, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Technicians : null))
                .AfterMap((src, dest) => {
                    if (dest.Technicians == null)
                        dest.Technicians = new List<Employee>();
                })
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Intervention>()
                .ConstructUsing((src, context) => context.Mapper.Map<Intervention>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Intervention>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Intervention>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Intervention>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Way of Entry mappings

            CreateMap<WayOfEntry, WayOfEntryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "way-of-entries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<WayOfEntry, WayOfEntryDto.AttributesDto>();

            CreateMap<WayOfEntry, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<WayOfEntry, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "way-of-entries"));

            CreateMap<OneRelationship, WayOfEntry>()
                .ConstructUsing((src, context) => context.Mapper.Map<WayOfEntry>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<WayOfEntry>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<WayOfEntry>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, WayOfEntry>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Product unit mappings

            CreateMap<ProductUnit, ProductUnitDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "product-units"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<ProductUnit, ProductUnitAttributesDto>();

            CreateMap<ProductUnit, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<ProductUnit, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "product-units"));

            CreateMap<OneRelationship, ProductUnit>()
                .ConstructUsing((src, context) => context.Mapper.Map<ProductUnit>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<ProductUnit>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<ProductUnit>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, ProductUnit>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Visit mappings

            CreateMap<CalendarEvent, CalendarEventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "calendar-events"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<CalendarEvent, CalendarEventAttributesDto>().ReverseMap();

            CreateMap<CalendarEvent, CalendarEventRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<CalendarEvent, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "calendar-events"));

            CreateMap<CalendarEventRequestDto, CalendarEvent>()
                .ConstructUsing((src, context) => context.Mapper.Map<CalendarEvent>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Request : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, CalendarEvent>()
                .ConstructUsing((src, context) => context.Mapper.Map<CalendarEvent>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<CalendarEvent>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<CalendarEvent>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, CalendarEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Offer mappings

            CreateMap<Offer, OfferDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "offers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Offer, OfferAttributesDto>().ReverseMap();

            CreateMap<Offer, OfferRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Offer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "offers"));

            CreateMap<OfferRequestDto, Offer>()
                .ConstructUsing((src, context) => context.Mapper.Map<Offer>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Request : null))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Order : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Offer>()
                .ConstructUsing((src, context) => context.Mapper.Map<Offer>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Offer>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Offer>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Offer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Offerline mappings

            CreateMap<Offerline, OfferlineDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "offerlines"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Offerline, OfferlineAttributesDto>().ReverseMap();

            CreateMap<Offerline, OfferlineRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Offerline, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "offerlines"));

            CreateMap<OfferlineRequestDto, Offerline>()
                .ConstructUsing((src, context) => context.Mapper.Map<Offerline>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Offer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Offer : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Offerline>()
                .ConstructUsing((src, context) => context.Mapper.Map<Offerline>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Offerline>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Offerline>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Offerline>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // VAT Rate mappings

            CreateMap<VatRate, VatRateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "vat-rates"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<VatRate, VatRateDto.AttributesDto>();

            CreateMap<VatRate, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<VatRate, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "vat-rates"));

            CreateMap<OneRelationship, VatRate>()
                .ConstructUsing((src, context) => context.Mapper.Map<VatRate>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<VatRate>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<VatRate>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, VatRate>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Order mappings

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "orders"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Order, OrderAttributesDto>().ReverseMap();

            CreateMap<Order, OrderRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Order, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "orders"));

            CreateMap<OrderRequestDto, Order>()
                .ConstructUsing((src, context) => context.Mapper.Map<Order>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.Offer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Offer : null))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForMember(dest => dest.Invoicelines, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoicelines : null))
                .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Deposits : null))
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.DepositInvoices : null))
                .ForMember(dest => dest.Interventions, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Interventions : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Order>()
                .ConstructUsing((src, context) => context.Mapper.Map<Order>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Order>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Order>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Invoiceline mappings

            CreateMap<Invoiceline, InvoicelineDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoicelines"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Invoiceline, InvoicelineAttributesDto>().ReverseMap();

            CreateMap<Invoiceline, InvoicelineRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Invoiceline, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoicelines"));

            CreateMap<InvoicelineRequestDto, Invoiceline>()
                .ConstructUsing((src, context) => context.Mapper.Map<Invoiceline>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Order : null))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, Invoiceline>()
                .ConstructUsing((src, context) => context.Mapper.Map<Invoiceline>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Invoiceline>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Invoiceline>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Invoiceline>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());


            // Invoice mappings

            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoices"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Invoice, InvoiceAttributesDto>().ReverseMap();

            CreateMap<Invoice, InvoiceRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Invoice, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoices"));

            CreateMap<InvoiceRequestDto, Invoice>()
                .ConstructUsing((src, context) => context.Mapper.Map<Invoice>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Order : null))
                .ForMember(dest => dest.Intervention, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Intervention : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForMember(dest => dest.Deposits, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Deposits : null))
                .ForMember(dest => dest.DepositInvoices, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.DepositInvoices : null))
                .ForMember(dest => dest.Supplements, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Supplements : null))
                .ForMember(dest => dest.Invoicelines, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoicelines : null))
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.WorkingHours : null))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "deposit-invoices"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<DepositInvoice, DepositInvoiceAttributesDto>().ReverseMap();

            CreateMap<DepositInvoice, DepositInvoiceRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<DepositInvoice, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "deposit-invoices"));

            CreateMap<DepositInvoiceRequestDto, DepositInvoice>()
                .ConstructUsing((src, context) => context.Mapper.Map<DepositInvoice>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Contact : null))
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Building : null))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Order : null))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.VatRate : null))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoice-supplements"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<InvoiceSupplement, InvoiceSupplementAttributesDto>().ReverseMap();

            CreateMap<InvoiceSupplement, InvoiceSupplementRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<InvoiceSupplement, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "invoice-supplements"));

            CreateMap<InvoiceSupplementRequestDto, InvoiceSupplement>()
                .ConstructUsing((src, context) => context.Mapper.Map<InvoiceSupplement>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Unit : null))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "deposits"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Deposit, DepositAttributesDto>().ReverseMap();

            CreateMap<Deposit, DepositRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Deposit, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "deposits"));

            CreateMap<DepositRequestDto, Deposit>()
                .ConstructUsing((src, context) => context.Mapper.Map<Deposit>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Customer : null))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Order : null))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Payment : null))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "employees"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Employee, EmployeeDto.AttributesDto>().ReverseMap();

            CreateMap<Employee, EmployeeDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Employee, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "employees"));

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "working-hours"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<WorkingHour, WorkingHourAttributesDto>().ReverseMap();

            CreateMap<WorkingHour, WorkingHourRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<WorkingHour, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "working-hours"));

            CreateMap<WorkingHourRequestDto, WorkingHour>()
                .ConstructUsing((src, context) => context.Mapper.Map<WorkingHour>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Invoice : null))
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Relationships != null ? src.Relationships.Employee : null))
                .ForAllOtherMembers(opt => opt.Ignore());

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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "payments"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Payment, PaymentDto.AttributesDto>();

            CreateMap<Payment, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Payment, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "payments"));

            CreateMap<OneRelationship, Payment>()
                .ConstructUsing((src, context) => context.Mapper.Map<Payment>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<Payment>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Payment>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, Payment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());;


            // Accountancy export mappings

            CreateMap<AccountancyExport, AccountancyExportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "accountancy-exports"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<AccountancyExport, AccountancyExportAttributesDto>().ReverseMap();

            CreateMap<AccountancyExport, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<AccountancyExport, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "accountancy-exports"));

            CreateMap<AccountancyExportRequestDto, AccountancyExport>()
                .ConstructUsing((src, context) => context.Mapper.Map<AccountancyExport>(src.Attributes))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OneRelationship, AccountancyExport>()
                .ConstructUsing((src, context) => context.Mapper.Map<AccountancyExport>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ManyRelationship, IEnumerable<AccountancyExport>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<AccountancyExport>>(src.Data))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RelatedResource, AccountancyExport>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opt => opt.Ignore());



            // Error notification mappings

            CreateMap<ErrorNotification, ErrorNotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "error-notifications"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<ErrorNotification, ErrorNotificationtAttributesDto>().ReverseMap();

            CreateMap<ErrorNotification, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<ErrorNotificationRequestDto, ErrorNotification>()
                .ConstructUsing((src, context) => context.Mapper.Map<ErrorNotification>(src.Attributes))
                .ForAllOtherMembers(opt => opt.Ignore());
        }

    }
}
