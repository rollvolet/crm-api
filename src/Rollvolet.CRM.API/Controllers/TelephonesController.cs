using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.APIContracts.DTO;
using Rollvolet.CRM.APIContracts.DTO.Buildings;
using Rollvolet.CRM.APIContracts.DTO.Contacts;
using Rollvolet.CRM.APIContracts.DTO.Customers;
using Rollvolet.CRM.APIContracts.DTO.Telephones;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;

namespace Rollvolet.CRM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TelephonesController : ControllerBase
    {
        private readonly ITelephoneManager _telephoneManager;
        private readonly ICountryManager _countryManager;
        private readonly ITelephoneTypeManager _telephoneTypeManager;
        private readonly ICustomerManager _customerManager;
        private readonly IContactManager _contactManager;
        private readonly IBuildingManager _buildingManager;
        private readonly IMapper _mapper;
        private readonly IJsonApiBuilder _jsonApiBuilder;

        public TelephonesController(ITelephoneManager telephoneManager, ICountryManager countryManager, ITelephoneTypeManager telephoneTypeManager,
                                    ICustomerManager customerManager, IContactManager contactManager, IBuildingManager buildingManager,
                                    IMapper mapper, IJsonApiBuilder jsonApiBuilder)
        {
            _telephoneManager = telephoneManager;
            _countryManager = countryManager;
            _telephoneTypeManager = telephoneTypeManager;
            _customerManager = customerManager;
            _contactManager = contactManager;
            _buildingManager = buildingManager;
            _mapper = mapper;
            _jsonApiBuilder = jsonApiBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ResourceRequest<TelephoneRequestDto> resource)
        {
            if (resource.Data.Type != "telephones") return StatusCode(409);

            var telephone = _mapper.Map<Telephone>(resource.Data);

            telephone = await _telephoneManager.CreateAsync(telephone);
            var telephoneDto = _mapper.Map<TelephoneDto>(telephone);

            var links = _jsonApiBuilder.BuildNewSingleResourceLinks(HttpContext.Request.Path, telephoneDto.Id);

            return Created(links.Self, new ResourceResponse() { Links = links, Data = telephoneDto });
        }

        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] ResourceRequest<TelephoneRequestDto> resource)
        {
            throw new NotSupportedException();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _telephoneManager.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("{telephoneId}/country")]
        [HttpGet("{telephoneId}/links/country")]
        public async Task<IActionResult> GetRelatedCountryByIdAsync(string telephoneId)
        {
            CountryDto countryDto;
            try
            {
                var country = await _countryManager.GetByTelephoneIdAsync(telephoneId);
                countryDto = _mapper.Map<CountryDto>(country);
            }
            catch (EntityNotFoundException)
            {
                countryDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = countryDto });
        }

        [HttpGet("{telephoneId}/telephone-type")]
        [HttpGet("{telephoneId}/links/telephone-type")]
        public async Task<IActionResult> GetRelatedTelephoneTypeByIdAsync(string telephoneId)
        {
            TelephoneTypeDto telephoneTypeDto;
            try
            {
                var telephoneType = await _telephoneTypeManager.GetByTelephoneIdAsync(telephoneId);
                telephoneTypeDto = _mapper.Map<TelephoneTypeDto>(telephoneType);
            }
            catch (EntityNotFoundException)
            {
                telephoneTypeDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = telephoneTypeDto });
        }

        [HttpGet("{telephoneId}/customer")]
        [HttpGet("{telephoneId}/links/customer")]
        public async Task<IActionResult> GetRelatedCustomerByIdAsync(string telephoneId)
        {
            CustomerDto customerDto;
            try
            {
                var customer = await _customerManager.GetByTelephoneIdAsync(telephoneId);
                customerDto = _mapper.Map<CustomerDto>(customer);
            }
            catch (EntityNotFoundException)
            {
                customerDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = customerDto });
        }

        [HttpGet("{telephoneId}/contact")]
        [HttpGet("{telephoneId}/links/contact")]
        public async Task<IActionResult> GetRelatedContactByIdAsync(string telephoneId)
        {
            ContactDto contactDto;
            try
            {
                var contact = await _contactManager.GetByTelephoneIdAsync(telephoneId);
                contactDto = _mapper.Map<ContactDto>(contact);
            }
            catch (EntityNotFoundException)
            {
                contactDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = contactDto });
        }

        [HttpGet("{telephoneId}/building")]
        [HttpGet("{telephoneId}/links/building")]
        public async Task<IActionResult> GetRelatedBuildingByIdAsync(string telephoneId)
        {
            BuildingDto buildingDto;
            try
            {
                var building = await _buildingManager.GetByTelephoneIdAsync(telephoneId);
                buildingDto = _mapper.Map<BuildingDto>(building);
            }
            catch (EntityNotFoundException)
            {
                buildingDto = null;
            }

            var links = _jsonApiBuilder.BuildSingleResourceLinks(HttpContext.Request.Path);
            return Ok(new ResourceResponse() { Links = links, Data = buildingDto });
        }
    }
}