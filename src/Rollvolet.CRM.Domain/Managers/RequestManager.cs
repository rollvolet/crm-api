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
        private readonly ICalendarEventManager _calendarEventManager;
        private readonly ILogger _logger;

        public RequestManager(IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IVisitDataProvider visitDataProvider, IOfferDataProvider offerDataProvider,
                                IWayOfEntryDataProvider wayOfEntryDataProvider, ICalendarEventManager calendarEventManager, ILogger<RequestManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _visitDataProvider = visitDataProvider;
            _offerDataProvider = offerDataProvider;
            _wayOfEntryDataProvider = wayOfEntryDataProvider;
            _calendarEventManager = calendarEventManager;
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

        public async Task<Request> GetByOrderIdAsync(int orderId)
        {
            return await _requestDataProvider.GetByOrderIdAsync(orderId);
        }

        public async Task<Request> CreateAsync(Request request)
        {
            if (request.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Request cannot have an id on create.");
            if (request.RequestDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request-date is required.");
            if (request.Offer != null)
            {
                var message = "Offer cannot be added to a request on creation.";
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
            query.Include.Fields = new string[] { "customer", "way-of-entry", "building", "contact", "calendar-event" };
            var existingRequest = await _requestDataProvider.GetByIdAsync(request.Id, query);

            if (request.Id != existingRequest.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Request id cannot be updated.");
            if (request.RequestDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request-date is required.");

            await EmbedRelations(request, existingRequest);

            if (request.Offer != null && request.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required if an offer is attached to the request.");
            if (request.Contact != null && request.Contact.Customer != null && request.Contact.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {request.Contact.Id}.");
            if (request.Building != null && request.Building.Customer != null && request.Building.Customer.Id != request.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {request.Customer.Id}.");

            request = await _requestDataProvider.UpdateAsync(request);

            if (request.Comment != existingRequest.Comment)
                await SyncCalendarEventAsync(request.Id);

            return request;
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var offer = await _offerDataProvider.GetByRequestIdAsync(id);
                var message = $"Request {id} cannot be deleted because offer {offer.Id} is attached to it.";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch(EntityNotFoundException)
            {
                await _requestDataProvider.DeleteByIdAsync(id);
            }
        }

        public async Task SyncCalendarEventAsync(int requestId)
        {
            try
            {
                var calendarEvent = await _calendarEventManager.GetByRequestIdAsync(requestId);
                await _calendarEventManager.UpdateAsync(calendarEvent, true);
            }
            catch (EntityNotFoundException)
            {
                // No calendar event to update
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

                if (request.Customer != null)
                {
                    if (oldRequest != null && oldRequest.Customer != null && oldRequest.Customer.Id == request.Customer.Id)
                        request.Customer = oldRequest.Customer;
                    else
                        request.Customer = await _customerDataProvider.GetByNumberAsync(request.Customer.Id);
                }

                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };

                // Offer cannot be updated. Take offer of oldRequest on update.
                if (oldRequest != null)
                    request.Offer = oldRequest.Offer;
                else
                    request.Offer = null;

                // Contact can only be updated through CaseManager. Take contact of oldRequest on update.
                if (oldRequest != null)
                    request.Contact = oldRequest.Contact;
                else if (request.Contact != null)
                    request.Contact = await _contactDataProvider.GetByIdAsync(request.Contact.Id, includeCustomer);

                // Building can only be updated through CaseManager. Take building of oldRequest on update.
                if (oldRequest != null)
                    request.Building = oldRequest.Building;
                else if (request.Building != null)
                    request.Building = await _buildingDataProvider.GetByIdAsync(request.Building.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }
    }
}