using System.Collections.Generic;
using AutoMapper;
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
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetCustomerRelationships(src.Id)))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Country, opt => opt.Ignore())
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.Buildings, opt => opt.Ignore())
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
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetContactRelationships(src.Id)))
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
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetBuildingRelationships(src.Id)))
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

            CreateMap<Country, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("countries"))
                .ReverseMap();

            CreateMap<HonorificPrefix, HonorificPrefixDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<HonorificPrefix, HonorificPrefixDto.AttributesDto>()
                .ReverseMap();

            CreateMap<HonorificPrefix, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("honorific-prefixes"))
                .ReverseMap();

            CreateMap<Language, LanguageDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<Language, LanguageDto.AttributesDto>()
                .ReverseMap();

            CreateMap<Language, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("languages"))
                .ReverseMap();

            CreateMap<PostalCode, PostalCodeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetEmptyRelationships()))
                .ReverseMap();

            CreateMap<PostalCode, PostalCodeDto.AttributesDto>()
                .ReverseMap();

            CreateMap<PostalCode, RelationResource>()
                .ForMember(dest => dest.Type, opt => opt.UseValue("postal-codes"))
                .ReverseMap();
        }
  
        private IDictionary<string, Relationship> GetCustomerRelationships(int id)
        {
            var fields = new string[6] {"contacts", "buildings", "country", "honorific-prefix", "language", "postal-code"};

            return GetResourceRelationships("customers", id, fields);
        }

        private IDictionary<string, Relationship> GetContactRelationships(int id)
        {
            var fields = new string[3] {"country", "language", "postal-code"};

            return GetResourceRelationships("contacts", id, fields);
        }

        private IDictionary<string, Relationship> GetBuildingRelationships(int id)
        {
            var fields = new string[3] {"country", "language", "postal-code"};

            return GetResourceRelationships("buildings", id, fields);
        }

        private IDictionary<string, Relationship> GetResourceRelationships(string name, int id, string[] fields)
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
