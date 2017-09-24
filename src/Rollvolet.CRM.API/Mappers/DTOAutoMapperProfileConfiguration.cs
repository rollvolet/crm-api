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
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetCustomerRelationships(src.Id.ToString())))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => GetRelationship("country", src.Relationships)))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => GetRelationship("language", src.Relationships))) 
                .ForMember(dest => dest.HonorificPrefix, opt => opt.MapFrom(src => GetRelationship("honorific-prefix", src.Relationships)))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => GetRelationship("postal-code", src.Relationships)))
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.Buildings, opt => opt.Ignore())
                .ForMember(dest => dest.Telephones, opt => opt.Ignore())
                .ConstructUsing((src, context) => context.Mapper.Map<Customer>(src.Attributes));

            CreateMap<Customer, CustomerDto.AttributesDto>()
                .ReverseMap();
            
            CreateMap<Customer, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"))
                .ReverseMap();

            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetContactRelationships(src.Id.ToString())))
                .ReverseMap();
            
            CreateMap<Contact, ContactDto.AttributesDto>()
                .ReverseMap();
            
            CreateMap<Contact, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ReverseMap();

            CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetBuildingRelationships(src.Id.ToString())))
                .ReverseMap();

            CreateMap<Building, BuildingDto.AttributesDto>()
                .ReverseMap();
            
            CreateMap<Building, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ReverseMap();

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<Country, CountryDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Relationship, Country>()
                .ConstructUsing((src, context) => context.Mapper.Map<Country>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Country, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<HonorificPrefix, HonorificPrefixDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<HonorificPrefix, HonorificPrefixDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Relationship, HonorificPrefix>()
                .ConstructUsing((src, context) => context.Mapper.Map<HonorificPrefix>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<HonorificPrefix, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Language, LanguageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<Language, LanguageDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Relationship, Language>()
                .ConstructUsing((src, context) => context.Mapper.Map<Language>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Language, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<PostalCode, PostalCodeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<PostalCode, PostalCodeDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Relationship, PostalCode>()
                .ConstructUsing((src, context) => context.Mapper.Map<PostalCode>(((JObject) src.Data).ToObject<RelationResource>()))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<PostalCode, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Telephone, TelephoneDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetTelephoneRelationships(src.Id)))
                .ReverseMap();

            CreateMap<Telephone, TelephoneDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Telephone, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephones"))
                .ReverseMap();

            CreateMap<TelephoneType, TelephoneTypeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<TelephoneType, TelephoneTypeDto.AttributesDto>()
                .ReverseMap();

            CreateMap<TelephoneType, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("telephone-types"))
                .ReverseMap();
        }

        private Relationship GetRelationship(string key, IDictionary<string, Relationship> relationships)
        {
            return relationships.ContainsKey(key) ? relationships[key] : null;
        }
  
        private IDictionary<string, Relationship> GetCustomerRelationships(string id)
        {
            var fields = new string[7] {"contacts", "buildings", "country", "honorific-prefix", "language", "postal-code", "telephones"};

            return GetResourceRelationships("customers", id, fields);
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
