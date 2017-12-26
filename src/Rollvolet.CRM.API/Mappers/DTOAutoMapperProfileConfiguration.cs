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


            // Request mappings

            CreateMap<Request, RequestDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("requests"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<Request, RequestDto.AttributesDto>();

            CreateMap<Request, RequestDto.RelationshipsDto>().ConvertUsing<RelationshipsConverter>();


            // Way of Entry mappings

            CreateMap<WayOfEntry, WayOfEntryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("ways-of-entry"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => src));

            CreateMap<WayOfEntry, WayOfEntryDto.AttributesDto>();

            CreateMap<WayOfEntry, EmptyRelationshipsDto>().ConvertUsing<RelationshipsConverter>();


        }

    }
}
