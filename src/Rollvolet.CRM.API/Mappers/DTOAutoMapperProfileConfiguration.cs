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

            CreateMap<Customer, CustomerDto.RelationshipsDto>().ConvertUsing<CustomerDtoRelationshipsConverter>();

            CreateMap<Customer, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"));


            // Contact mappings
            
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetContactRelationships(src.Id.ToString())));
            
            CreateMap<Contact, ContactDto.AttributesDto>();

            CreateMap<Relationship, IEnumerable<Contact>>()
                .ConstructUsing((src, context) => context.Mapper.Map<IEnumerable<Contact>>(((JArray) src.Data).ToObject<List<RelationResource>>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Contact, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"));


            // Building mappings

            CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetBuildingRelationships(src.Id.ToString())));

            CreateMap<Building, BuildingDto.AttributesDto>();
            
            CreateMap<Building, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"));


            // Country mappings

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()));

            CreateMap<Country, CountryDto.AttributesDto>();

            CreateMap<Relationship, Country>()
                .ConstructUsing((src, context) => context.Mapper.Map<Country>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Country, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"));


            // Honorific prefix mappings

            CreateMap<HonorificPrefix, HonorificPrefixDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()));

            CreateMap<HonorificPrefix, HonorificPrefixDto.AttributesDto>();

            CreateMap<Relationship, HonorificPrefix>()
                .ConstructUsing((src, context) => context.Mapper.Map<HonorificPrefix>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<HonorificPrefix, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"));


            // Language mappings

            CreateMap<Language, LanguageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()));

            CreateMap<Language, LanguageDto.AttributesDto>();

            CreateMap<Relationship, Language>()
                .ConstructUsing((src, context) => context.Mapper.Map<Language>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Language, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"));


            // Postal code mappings

            CreateMap<PostalCode, PostalCodeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()));

            CreateMap<PostalCode, PostalCodeDto.AttributesDto>();

            CreateMap<Relationship, PostalCode>()
                .ConstructUsing((src, context) => context.Mapper.Map<PostalCode>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<PostalCode, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"));


            // Telephone mappings

            CreateMap<Telephone, TelephoneDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetTelephoneRelationships(src.Id)));

            CreateMap<Telephone, TelephoneDto.AttributesDto>();

            CreateMap<Telephone, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"));


            // Telephone type mappings

            CreateMap<TelephoneType, TelephoneTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()));

            CreateMap<TelephoneType, TelephoneTypeDto.AttributesDto>();

            CreateMap<TelephoneType, RelatedResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"));
        }

        private Relationship GetRelationship(string key, IDictionary<string, Relationship> relationships)
        {
            return relationships.ContainsKey(key) ? relationships[key] : null;
        }

        private IDictionary<string, Relationship> GetContactRelationships(string id)
        {
            var fields = new string[3] {"country", "language", "postal-code"};

            return GetResourceRelationships("contacts", id, fields);
        }

        private IDictionary<string, Relationship> GetBuildingRelationships(string id)
        {
            var fields = new string[3] {"country", "language", "postal-code"};

            return GetResourceRelationships("buildings", id, fields);
        }

        private IDictionary<string, Relationship> GetTelephoneRelationships(string id)
        {
            var fields = new string[2] {"country", "telephone-type"};

            return GetResourceRelationships("telephones", id, fields);
        }

        private IDictionary<string, Relationship> GetResourceRelationships(string name, string id, string[] fields)
        {
            var relationships = new Dictionary<string, Relationship>();
            
            foreach (var field in fields)
            {             
                relationships.Add(field, new Relationship() {
                    Links = new Links { Self = $"/{name}/{id}/links/{field}", Related = $"/{name}/{id}/{field}" }
                });   
            }
            
            return relationships;
        }

        private IDictionary<string, Relationship> GetEmptyRelationships()
        {
            return new Dictionary<string, Relationship>();
        }
    }
}
