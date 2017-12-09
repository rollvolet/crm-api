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
                                            ITypeConverter<Country, EmptyRelationshipsDto>,
                                            ITypeConverter<Language, EmptyRelationshipsDto>,
                                            ITypeConverter<PostalCode, EmptyRelationshipsDto>,
                                            ITypeConverter<TelephoneType, EmptyRelationshipsDto>,
                                            ITypeConverter<HonorificPrefix, EmptyRelationshipsDto>
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

        private IRelationship GetManyRelationship<T>(string resourceName, int id, string relatedResourceName, IEnumerable<T> relatedResources, ResolutionContext context)
        {
            return GetManyRelationship<T>(resourceName, id.ToString(), relatedResourceName, relatedResources, context);       
        }

        private IRelationship GetManyRelationship<T>(string resourceName, string id, string relatedResourceName, IEnumerable<T> relatedResources, ResolutionContext context)
        {
            if (context.Items.Keys.Contains("include") && !((IncludeQuery) context.Items["include"]).Contains(relatedResourceName))
            {
                return new Relationship() { Links = GetRelationshipLinks(resourceName, id, relatedResourceName) };
            }
            else
            {
                return new ManyRelationship() { 
                    Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                    Data = context.Mapper.Map<IEnumerable<RelatedResource>>(relatedResources)
                };
            }
        }

        private IRelationship GetOneRelationship<T>(string resourceName, int id, string relatedResourceName, T relatedResource, ResolutionContext context)
        {
            return GetOneRelationship<T>(resourceName, id.ToString(), relatedResourceName, relatedResource, context);
        }

        private IRelationship GetOneRelationship<T>(string resourceName, string id, string relatedResourceName, T relatedResource, ResolutionContext context)
        {
            if (context.Items.Keys.Contains("include") && !((IncludeQuery) context.Items["include"]).Contains(relatedResourceName))
            {
                return new Relationship() { Links = GetRelationshipLinks(resourceName, id, relatedResourceName) };
            }
            else
            {
                return new OneRelationship() { 
                    Links = GetRelationshipLinks(resourceName, id, relatedResourceName),
                    Data = context.Mapper.Map<RelatedResource>(relatedResource)
                };
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