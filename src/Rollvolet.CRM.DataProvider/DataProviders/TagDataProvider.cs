using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Narato.ResponseMiddleware.Models.Exceptions;
using Rollvolet.CRM.DataProvider.Contexts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.DataProvider.Extensions;
using Microsoft.Extensions.Logging;
using LinqKit;

namespace Rollvolet.CRM.DataProviders
{   
    public class TagDataProvider : ITagDataProvider
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TagDataProvider(CrmContext context, IMapper mapper, ILogger<TagDataProvider> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Paged<Tag>> GetAllByCustomerNumberAsync(int customerNumber, QuerySet query)
        {
            var dataId = await _context.Customers.Where(c => c.Number == customerNumber).Select(c => c.DataId).FirstOrDefaultAsync();

            if (dataId == 0)
            {
                // TODO implement and handle exceptions according to jsonapi
                _logger.LogError($"No customer found with number {customerNumber}");
                throw new EntityNotFoundException("ENF", $"Customer with number {customerNumber} not found");
            }

            var source = _context.CustomerTags
                            .Where(t => t.CustomerId == dataId)
                            .Include(e => e.Tag)
                            .OrderBy(e => e.Tag.Name)
                            .Select(e => e.Tag);

            var tags = source.Skip(query.Page.Skip).Take(query.Page.Take).AsEnumerable();                            

            var mappedTags = _mapper.Map<IEnumerable<Tag>>(tags);

            var count = await source.CountAsync();

            return new Paged<Tag>() {
                Items = mappedTags,
                Count = count,
                PageNumber = query.Page.Number,
                PageSize = query.Page.Size
            };            
        }    
    }
}