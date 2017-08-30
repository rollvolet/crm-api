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
                .ReverseMap();

            CreateMap<Customer, Resource<CustomerDto>>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("customers"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetCustomerRelationships(src.Id)))
                .ReverseMap();
            
            CreateMap<Contact, ContactDto>()
                .ReverseMap();

            CreateMap<Contact, Resource<ContactDto>>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("contacts"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetContactRelationships(src.Id)))
                .ReverseMap();
                            
            CreateMap<Building, BuildingDto>()
                .ReverseMap();

            CreateMap<Building, Resource<BuildingDto>>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.UseValue("buildings"))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Relationships, opt => opt.MapFrom(src => GetBuildingRelationships(src.Id)))
                .ReverseMap();
                
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ReverseMap();
        }
  
        private object GetCustomerRelationships(int id)
        {
            return new {
                Contacts = new { Links = new { Self = $"/customers/{id}/relationships/contacts" }},
                Buildings = new { Links = new { Self = $"/customers/{id}/relationships/buildings" }}
            };
        }

        private object GetContactRelationships(int id)
        {
            return new { };
        }

        private object GetBuildingRelationships(int id)
        {
            return new { };
        }
    }
}
