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
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IVisitDataProvider _visitDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IWayOfEntryDataProvider _wayOfEntryDataProvider;
        private readonly IVisitManager _visitManager;
        private readonly ILogger _logger;

        public RequestManager(IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IVisitDataProvider visitDataProvider, IOfferDataProvider offerDataProvider,
                                IWayOfEntryDataProvider wayOfEntryDataProvider, IVisitManager visitManager, ILogger<RequestManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _visitDataProvider = visitDataProvider;
            _offerDataProvider = offerDataProvider;
            _wayOfEntryDataProvider = wayOfEntryDataProvider;
            _visitManager = visitManager;
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

        public async Task<Request> GetByOfferIdAsync(int offerId)
        {
            return await _requestDataProvider.GetByOfferIdAsync(offerId);
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
            if (request.Visit != null)
            {
                var message = "Visit cannot be added to a request on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(request);

            if (request.Contact != null && request.Contact.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {request.Contact.Id}.");
            if (request.Building != null && request.Building.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {request.Customer.Id}.");

            return await _requestDataProvider.CreateAsync(request);
        }

        public async Task<Request> UpdateAsync(Request request)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "way-of-entry", "building", "contact", "visit" };
            var existingRequest = await _requestDataProvider.GetByIdAsync(request.Id, query);

            if (request.Id != existingRequest.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Request id cannot be updated.");
            if (request.RequestDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request-date is required.");

            await EmbedRelations(request, existingRequest);

            if (request.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (request.Contact != null && request.Contact.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {request.Contact.Id}.");
            if (request.Building != null && request.Building.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {request.Customer.Id}.");

            request = await _requestDataProvider.UpdateAsync(request);

            if (request.Comment != existingRequest.Comment)
                await SyncVisit(request);

            return request;
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var offer = await _offerDataProvider.GetByRequestIdAsync(id);
                _logger.LogError($"Request {id} cannot be deleted because offer {offer.Id} is attached to it.");
                throw new InvalidOperationException($"Request {id} cannot be deleted because offer {offer.Id} is attached to it.");
            }
            catch(EntityNotFoundException)
            {
                await _requestDataProvider.DeleteByIdAsync(id);
            }
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

                if (request.Contact != null)
                {
                    if (oldRequest != null && oldRequest.Contact != null && oldRequest.Contact.Id == request.Contact.Id)
                        request.Contact = oldRequest.Contact;
                    else
                        request.Contact = await _contactDataProvider.GetByIdAsync(request.Contact.Id);
                }

                if (request.Building != null)
                {
                    if (oldRequest != null && oldRequest.Building != null && oldRequest.Building.Id == request.Building.Id)
                        request.Building = oldRequest.Building;
                    else
                        request.Building = await _buildingDataProvider.GetByIdAsync(request.Building.Id);
                }

                if (request.Visit != null)
                {
                    if (oldRequest != null && oldRequest.Visit != null && oldRequest.Visit.Id == request.Visit.Id)
                        request.Visit = oldRequest.Visit;
                    else
                        request.Visit = await _visitDataProvider.GetByIdAsync(request.Visit.Id);
                }

                // Customer cannot be updated. Take customer of oldRequest on update.
                if (oldRequest != null)
                    request.Customer = oldRequest.Customer;
                else
                    request.Customer = await _customerDataProvider.GetByNumberAsync(request.Customer.Id);

                // Offer cannot be updated. Take offer of oldRequest on update.
                if (oldRequest != null)
                    request.Offer = oldRequest.Offer;
                else
                    request.Offer = await _offerDataProvider.GetByIdAsync(request.Offer.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

        private async Task SyncVisit(Request request)
        {
            try
            {
                var visit = await _visitManager.GetByRequestIdAsync(request.Id);
                await _visitManager.UpdateAsync(visit);
            }
            catch(EntityNotFoundException)
            {
                // No visit to update
            }
        }
    }
}