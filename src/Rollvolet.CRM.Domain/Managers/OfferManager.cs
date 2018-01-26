using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class OfferManager : IOfferManager
    {
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly ILogger _logger;

        public OfferManager(IOfferDataProvider offerDataProvider, ILogger<OfferManager> logger)
        {
            _offerDataProvider = offerDataProvider;
            _logger = logger;
        }
        
        public async Task<Paged<Offer>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "offer-date";
            }

            return await _offerDataProvider.GetAllAsync(query);
        }

        public async Task<Offer> GetByIdAsync(int id, QuerySet query)
        {
            return await _offerDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Offer>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null) {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "offer-date";
            }

            return await _offerDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }
    }
}