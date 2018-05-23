using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class RequestManager : IRequestManager
    {
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IWayOfEntryDataProvider _wayOfEntryDataProvider;
        private readonly ILogger _logger;

        public RequestManager(IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                                IWayOfEntryDataProvider wayOfEntryDataProvider, ILogger<RequestManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _wayOfEntryDataProvider = wayOfEntryDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Request>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "request-date";
            }

            return await _requestDataProvider.GetAllAsync(query);
        }

        public async Task<Request> GetByIdAsync(int id, QuerySet query)
        {
            return await _requestDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Request>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "request-date";
            }

            return await _requestDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Request> CreateAsync(Request request)
        {
            if (request.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Request cannot have an id on create.");
            if (request.RequestDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request-date is required.");
            if (request.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on request creation.");
            if (request.Offer != null)
            {
                var message = "Offer cannot be added to a request on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(request);

            return await _requestDataProvider.CreateAsync(request);
        }

        public async Task<Request> UpdateAsync(Request request)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "way-of-entry", "building", "contact" };
            var existingRequest = await _requestDataProvider.GetByIdAsync(request.Id, query);

            if (request.Id != existingRequest.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Request id cannot be updated.");
            if (request.RequestDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request-date is required.");
            if (request.Offer != null)
            {
                var message = "Offer cannot be changed during offer update.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(request, existingRequest);

            if (request.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");

            return await _requestDataProvider.UpdateAsync(request);
        }

        public async Task DeleteAsync(int id)
        {
            await _requestDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in request resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Request request, Request oldRequest = null)
        {
            try {
                if (request.WayOfEntry != null)
                {
                    if (oldRequest != null && oldRequest.WayOfEntry != null && oldRequest.WayOfEntry.Id == request.WayOfEntry.Id)
                        request.WayOfEntry = oldRequest.WayOfEntry;
                    else
                        request.WayOfEntry = await _wayOfEntryDataProvider.GetByIdAsync(int.Parse(request.WayOfEntry.Id));
                }

                // TODO handle building/contact

                // TODO handle visit

                // Customer cannot be updated. Take customer of oldBuilding on update.
                if (oldRequest != null)
                    request.Customer = oldRequest.Customer;
                else
                    request.Customer = await _customerDataProvider.GetByNumberAsync(request.Customer.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}