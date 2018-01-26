using System;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Rollvolet.CRM.APIContracts.DTO;
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
                //.ReverseMap()
                // .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                // .ForMember(dest => dest.Country, opt => opt.MapFrom(src => GetRelationship("country", src.Relationships)))
                // .ForMember(dest => dest.Language, opt => opt.MapFrom(src => GetRelationship("language", src.Relationships))) 
                // .ForMember(dest => dest.HonorificPrefix, opt => opt.MapFrom(src => GetRelationship("honorific-prefix", src.Relationships)))
                // .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => GetRelationship("postal-code", src.Relationships)))
                // .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => GetRelationship("contacts", src.Relationships)))
                // .ForMember(dest => dest.Buildings, opt => opt.Ignore())
                // .ForMember(dest => dest.Telephones, opt => opt.Ignore())
                //.ConstructUsing((src, context) => context.Mapper.Map<Customer>(src.Attributes));

            CreateMap<Customer, CustomerDto.AttributesDto>();

            CreateMap<Customer, CustomerDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Customer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"));


            // Contact mappings
            
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));
            
            CreateMap<Contact, ContactDto.AttributesDto>();

            CreateMap<Contact, ContactDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Contact, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"));


            // Building mappings

            CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Building, BuildingDto.AttributesDto>();

            CreateMap<Building, BuildingDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();
            
            CreateMap<Building, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"));


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


            // Telephone mappings

            CreateMap<Telephone, TelephoneDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Telephone, TelephoneDto.AttributesDto>();

            CreateMap<Telephone, TelephoneDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Telephone, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"));


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


            // Product mappings

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("products"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Product, ProductDto.AttributesDto>();

            CreateMap<Product, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();

            CreateMap<Product, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("products"));                                       
        }

    }
}
