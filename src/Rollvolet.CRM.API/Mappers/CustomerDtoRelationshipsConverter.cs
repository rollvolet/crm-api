using System.Collections.Generic;
using AutoMapper;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Mappers
{
    public class CustomerDtoRelationshipsConverter : ITypeConverter<Customer, CustomerDto.RelationshipsDto>
    {
        CustomerDto.RelationshipsDto ITypeConverter<Customer, CustomerDto.RelationshipsDto>.Convert(Customer source, CustomerDto.RelationshipsDto destination, ResolutionContext context)
        {
            var relationships = new CustomerDto.RelationshipsDto();
            relationships.Contacts = GetManyRelationship<Contact>("customers", source.Id, "contacts", source.Contacts, context);
            relationships.Buildings = GetManyRelationship<Building>("customers", source.Id, "buildings", source.Buildings, context);
            relationships.Country = GetOneRelationship<Country>("customers", source.Id, "country", source.Country, context);
            relationships.Language = GetOneRelationship<Language>("customers", source.Id, "language", source.Language, context);
            relationships.PostalCode = GetOneRelationship<PostalCode>("customers", source.Id, "postal-code", source.PostalCode, context);
            relationships.HonorificPrefix = GetOneRelationship<HonorificPrefix>("customers", source.Id, "honorific-prefix", source.HonorificPrefix, context);
            relationships.Telephones = GetManyRelationship<Telephone>("customers", source.Id, "telephones", source.Telephones, context);
            return relationships;
        }

        private ManyRelationship GetManyRelationship<T>(string resourceName, int id, string relatedResourceName, IEnumerable<T> relatedResources, ResolutionContext context)
        {
            return new ManyRelationship() { 
                Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                Data = context.Mapper.Map<IEnumerable<RelatedResource>>(relatedResources)
            };          
        }

        private OneRelationship GetOneRelationship<T>(string resourceName, int id, string relatedResourceName, T relatedResource, ResolutionContext context)
        {
            return new OneRelationship() { 
                Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                Data = context.Mapper.Map<RelatedResource>(relatedResource)
            };          
        }

        private Links GetRelationshipLinks(string resourceName, int id, string relationName) {
            return new Links { 
                Self = $"/{resourceName}/{id}/links/{relationName}", 
                Related = $"/{resourceName}/{id}/{relationName}"
            };
        }
  }
}