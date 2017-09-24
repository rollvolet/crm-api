using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.API.Controllers
{
    public class JsonApiController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly IJsonApiBuilder _jsonApiBuilder;

        public JsonApiController(IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        protected void MapOneAndUpdateIncluded<T, U>(string key, T value, QuerySet querySet, Resource resource, ISet<Resource> included) where U : Resource
        {
            if (querySet.Include.Contains(key) && value != null)
            {
                resource.Relationships[key].Data = _mapper.Map<RelationResource>(value);
                included.Add(_mapper.Map<U>(value));
            }
        }

        protected void MapManyAndUpdateIncluded<T, U>(string key, IEnumerable<T> value, QuerySet querySet, Resource resource, ISet<Resource> included) where U : Resource
        {
            if (querySet.Include.Contains(key) && value.Count() > 0)
            {
                resource.Relationships[key].Data = _mapper.Map<IEnumerable<RelationResource>>(value);
                included.UnionWith(_mapper.Map<IEnumerable<U>>(value));
            }
        }
    }
}