using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.APIContracts.DTO.ProductUnits;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Domain.Managers.Interfaces;

namespace Rollvolet.CRM.API.Controllers
{
    [Route("product-units")]
    [Authorize]
    public class ProductUnitsController : ControllerBase
    {
        private readonly IProductUnitManager _productUnitManager;
        private readonly IMapper _mapper;

        public ProductUnitsController(IProductUnitManager productUnitManager, IMapper mapper)
        {
            _productUnitManager = productUnitManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var productUnits = await _productUnitManager.GetAllAsync();

            var productUnitDtos = _mapper.Map<IEnumerable<ProductUnitDto>>(productUnits);
            var links = new CollectionLinks() { Self = HttpContext.Request.Path };

            return Ok(new ResourceResponse() { Links = links, Data = productUnitDtos });
        }
    }
}