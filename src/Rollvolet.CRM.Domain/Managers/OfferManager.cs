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
    public class OfferManager : IOfferManager
    {
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IVatRateDataProvider _vatRateDataProvider;
        private readonly ISubmissionTypeDataProvider _submissionTypeDataProvider;
        private readonly ILogger _logger;

        public OfferManager(IOfferDataProvider offerDataProvider, IRequestDataProvider requestDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IVatRateDataProvider vatRateDataProvider, ISubmissionTypeDataProvider submissionTypeDataProvider, ILogger<OfferManager> logger)
        {
            _offerDataProvider = offerDataProvider;
            _requestDataProvider = requestDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _vatRateDataProvider = vatRateDataProvider;
            _submissionTypeDataProvider = submissionTypeDataProvider;
            _logger = logger;
        }

        public async Task<Paged<Offer>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
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
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "offer-date";
            }

            return await _offerDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Offer> CreateAsync(Offer offer)
        {
            if (offer.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Offer cannot have an id on create.");
            if (offer.Number != null || offer.SequenceNumber != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Offer cannot have a number/sequence-number on create.");
            if (offer.OfferDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer-date is required.");
            if (offer.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required on offer creation.");
            if (offer.Request == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request is required on offer creation.");
            if (offer.Order != null)
            {
                var message = "Order cannot be added to an offer on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(offer);

            if (offer.Contact != null && offer.Contact.Customer.Id != offer.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {offer.Contact.Id}.");
            if (offer.Building != null && offer.Building.Customer.Id != offer.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {offer.Customer.Id}.");

            return await _offerDataProvider.CreateAsync(offer);
        }

        public async Task<Offer> UpdateAsync(Offer offer)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "building", "contact", "request", "submission-type", "vat-rate" };
            var existingRequest = await _offerDataProvider.GetByIdAsync(offer.Id, query);

            if (offer.Id != existingRequest.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Offer id cannot be updated.");
            if (offer.Number != existingRequest.Number)
                throw new IllegalArgumentException("IllegalAttribute", "Offer number cannot be updated.");
            if (offer.SequenceNumber != existingRequest.SequenceNumber)
                throw new IllegalArgumentException("IllegalAttribute", "Offer sequence-number cannot be updated.");
            if (offer.OfferDate == null)
                throw new IllegalArgumentException("IllegalAttribute", "Offer-date is required.");
            if (offer.Order != null)
            {
                var message = "Order cannot be changed during request update.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(offer, existingRequest);

            if (offer.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required.");
            if (offer.Request == null)
                throw new IllegalArgumentException("IllegalAttribute", "Request is required.");
            if (offer.Contact != null && offer.Contact.Customer.Id != offer.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {offer.Contact.Id}.");
            if (offer.Building != null && offer.Building.Customer.Id != offer.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {offer.Customer.Id}.");

            return await _offerDataProvider.UpdateAsync(offer);
        }

        public async Task DeleteAsync(int id)
        {
            await _offerDataProvider.DeleteByIdAsync(id);
        }

        // Embed relations in request resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Offer offer, Offer oldOffer = null)
        {
            try {
                if (offer.VatRate != null)
                {
                    if (oldOffer != null && oldOffer.VatRate != null && oldOffer.VatRate.Id == offer.VatRate.Id)
                        offer.VatRate = oldOffer.VatRate;
                    else
                        offer.VatRate = await _vatRateDataProvider.GetByIdAsync(int.Parse(offer.VatRate.Id));
                }

                if (offer.SubmissionType != null)
                {
                    if (oldOffer != null && oldOffer.SubmissionType != null && oldOffer.SubmissionType.Id == offer.SubmissionType.Id)
                        offer.SubmissionType = oldOffer.SubmissionType;
                    else
                        offer.SubmissionType = await _submissionTypeDataProvider.GetByIdAsync(offer.SubmissionType.Id);
                }

                // TODO prevent update of contact/building. Needs to bubble to request, order, etc.
                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };
                if (offer.Contact != null)
                {
                    if (oldOffer != null && oldOffer.Contact != null && oldOffer.Contact.Id == offer.Contact.Id)
                        offer.Contact = oldOffer.Contact;
                    else
                        offer.Contact = await _contactDataProvider.GetByIdAsync(offer.Contact.Id, includeCustomer);
                }

                if (offer.Building != null)
                {
                    if (oldOffer != null && oldOffer.Building != null && oldOffer.Building.Id == offer.Building.Id)
                        offer.Building = oldOffer.Building;
                    else
                        offer.Building = await _buildingDataProvider.GetByIdAsync(offer.Building.Id, includeCustomer);
                }

                // Customer cannot be updated. Take customer of oldOffer on update.
                if (oldOffer != null)
                    offer.Customer = oldOffer.Customer;
                else
                    offer.Customer = await _customerDataProvider.GetByNumberAsync(offer.Customer.Id);

                // Request cannot be updated. Take customer of oldOffer on update.
                if (oldOffer != null)
                    offer.Request = oldOffer.Request;
                else
                    offer.Request = await _requestDataProvider.GetByIdAsync(offer.Request.Id);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

    }
}