using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class InterventionManager : IInterventionManager
    {
        private readonly IInterventionDataProvider _interventionDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IWayOfEntryDataProvider _wayOfEntryDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly IPlanningEventManager _planningEventManager;
        private readonly ILogger _logger;

        public InterventionManager(IInterventionDataProvider interventionDataProvider, ICustomerDataProvider customerDataProvider,
                                IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                IInvoiceDataProvider invoiceDataProvider, IOrderDataProvider orderDataProvider,
                                 IRequestDataProvider requestDataProvider,
                                IWayOfEntryDataProvider wayOfEntryDataProvider, IEmployeeDataProvider employeeDataProvider,
                                IPlanningEventManager planningEventManager, ILogger<InterventionManager> logger)
        {
            _interventionDataProvider = interventionDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _orderDataProvider = orderDataProvider;
            _requestDataProvider = requestDataProvider;
            _wayOfEntryDataProvider = wayOfEntryDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _planningEventManager = planningEventManager;
            _logger = logger;
        }

        public async Task<Paged<Intervention>> GetAllAsync(QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _interventionDataProvider.GetAllAsync(query);
        }

        public async Task<Intervention> GetByIdAsync(int id, QuerySet query)
        {
            return await _interventionDataProvider.GetByIdAsync(id, query);
        }

        public async Task<Paged<Intervention>> GetAllByCustomerIdAsync(int customerId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _interventionDataProvider.GetAllByCustomerIdAsync(customerId, query);
        }

        public async Task<Paged<Intervention>> GetAllByContactIdAsync(int contactId, QuerySet query)
        {
            try
            {
                var includeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "customer" };
                var contact = await _contactDataProvider.GetByIdAsync(contactId, includeQuery);

                if (query.Sort.Field == null)
                {
                    query.Sort.Order = SortQuery.ORDER_DESC;
                    query.Sort.Field = "number";
                }

                return await _interventionDataProvider.GetAllByRelativeContactIdAsync(contact.Customer.Id, contact.Number, query);
            }
            catch (EntityNotFoundException)
            {
                return new Paged<Intervention> {
                    Count = 0,
                    Items = new List<Intervention>(),
                    PageNumber = 0,
                    PageSize = query.Page.Size
                };
            }
        }

        public async Task<Paged<Intervention>> GetAllByBuildingIdAsync(int buildingId, QuerySet query)
        {
            try
            {
                var includeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "customer" };
                var building = await _buildingDataProvider.GetByIdAsync(buildingId, includeQuery);

                if (query.Sort.Field == null)
                {
                    query.Sort.Order = SortQuery.ORDER_DESC;
                    query.Sort.Field = "number";
                }

                return await _interventionDataProvider.GetAllByRelativeBuildingIdAsync(building.Customer.Id, building.Number, query);
            }
            catch (EntityNotFoundException)
            {
                return new Paged<Intervention> {
                    Count = 0,
                    Items = new List<Intervention>(),
                    PageNumber = 0,
                    PageSize = query.Page.Size
                };
            }
        }

        public async Task<Paged<Intervention>> GetAllByOrderIdAsync(int orderId, QuerySet query)
        {
            if (query.Sort.Field == null)
            {
                query.Sort.Order = SortQuery.ORDER_DESC;
                query.Sort.Field = "date";
            }

            return await _interventionDataProvider.GetAllByOrderIdAsync(orderId, query);
        }

        public async Task<Intervention> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _interventionDataProvider.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<Intervention> GetByFollowUpRequestIdAsync(int requestId)
        {
            return await _interventionDataProvider.GetByFollowUpRequestIdAsync(requestId);
        }

        public async Task<Intervention> GetByPlanningEventIdAsync(int planningEventId)
        {
            return await _interventionDataProvider.GetByPlanningEventIdAsync(planningEventId);
        }

        public async Task<Intervention> CreateAsync(Intervention intervention)
        {
            if (intervention.Id != 0)
                throw new IllegalArgumentException("IllegalAttribute", "Intervention cannot have an id on create.");
            if (intervention.Date == null)
                throw new IllegalArgumentException("IllegalAttribute", "Intervention-date is required.");
            if (intervention.Invoice != null)
            {
                var message = "Invoice cannot be added to an intervention on creation.";
                _logger.LogDebug(message);
                throw new IllegalArgumentException("IllegalAttribute", message);
            }

            await EmbedRelations(intervention);

            if (intervention.Contact != null && intervention.Contact.Customer.Id != intervention.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {intervention.Contact.Id}.");
            if (intervention.Building != null && intervention.Building.Customer.Id != intervention.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {intervention.Customer.Id}.");

            return await _interventionDataProvider.CreateAsync(intervention);
        }

        public async Task<Intervention> UpdateAsync(Intervention intervention)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer", "way-of-entry", "building", "contact", "invoice", "origin", "technicians", "planning-event" };
            var existingIntervention = await _interventionDataProvider.GetByIdAsync(intervention.Id, query);

            if (intervention.Id != existingIntervention.Id)
                throw new IllegalArgumentException("IllegalAttribute", "Intervention id cannot be updated.");
            if (intervention.Date == null)
                throw new IllegalArgumentException("IllegalAttribute", "Intervention-date is required.");

            await EmbedRelations(intervention, existingIntervention);

            if (intervention.Invoice != null && intervention.Customer == null)
                throw new IllegalArgumentException("IllegalAttribute", "Customer is required if an invoice is attached to the intervention.");
            if (intervention.Contact != null && intervention.Contact.Customer != null && intervention.Contact.Customer.Id != intervention.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Contact is not attached to customer {intervention.Contact.Id}.");
            if (intervention.Building != null && intervention.Building.Customer != null && intervention.Building.Customer.Id != intervention.Customer.Id)
                throw new IllegalArgumentException("IllegalAttribute", $"Building is not attached to customer {intervention.Customer.Id}.");

            intervention = await _interventionDataProvider.UpdateAsync(intervention);

            if (intervention.PlanningEvent.IsPlanned && RequiresPlanningEventUpdate(existingIntervention, intervention))
                await _planningEventManager.UpdateAsync(intervention.PlanningEvent);

            return intervention;
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var invoice = await _invoiceDataProvider.GetByInterventionIdAsync(id);
                var message = $"Intervention {id} cannot be deleted because invoice {invoice.Id} is attached to it.";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            catch(EntityNotFoundException)
            {
                await _interventionDataProvider.DeleteByIdAsync(id);
            }
        }

        // Embed relations in intervention resource: reuse old relation if there is one and it hasn't changed
        private async Task EmbedRelations(Intervention intervention, Intervention oldIntervention = null)
        {
            try {
                if (intervention.WayOfEntry != null)
                {
                    if (oldIntervention != null && oldIntervention.WayOfEntry != null && oldIntervention.WayOfEntry.Id == intervention.WayOfEntry.Id)
                        intervention.WayOfEntry = oldIntervention.WayOfEntry;
                    else
                        intervention.WayOfEntry = await _wayOfEntryDataProvider.GetByIdAsync(int.Parse(intervention.WayOfEntry.Id));
                }

                if (intervention.Employee != null)
                {
                    if (oldIntervention != null && oldIntervention.Employee != null && oldIntervention.Employee.Id == intervention.Employee.Id)
                        intervention.Employee = oldIntervention.Employee;
                    else
                        intervention.Employee = await _employeeDataProvider.GetByIdAsync(intervention.Employee.Id);
                }

                if (intervention.Origin != null)
                {
                    if (oldIntervention != null && oldIntervention.Origin != null && oldIntervention.Origin.Id == intervention.Origin.Id)
                        intervention.Origin = oldIntervention.Origin;
                    else
                        intervention.Origin = await _orderDataProvider.GetByIdAsync(intervention.Origin.Id);
                }

                var technicians = new List<Employee>();
                foreach (var technicianRelation in intervention.Technicians)
                {
                    var technician = await _employeeDataProvider.GetByIdAsync(technicianRelation.Id);
                    technicians.Add(technician);
                }
                intervention.Technicians = technicians;

                // Planning-event cannot be updated. Take planning-event of oldIntervention on update.
                if (oldIntervention != null)
                    intervention.PlanningEvent = oldIntervention.PlanningEvent;
                else
                    intervention.PlanningEvent = null;

                // Invoice cannot be updated. Take invoice of oldRequest on update.
                if (oldIntervention != null)
                    intervention.Invoice = oldIntervention.Invoice;
                else
                    intervention.Invoice = null;

                if (intervention.Invoice != null && intervention.Customer == null)
                {
                    intervention.Customer = oldIntervention.Customer;  // Intervention already has an invoice, so it must have a customer
                }
                else
                {
                    if (intervention.Customer != null)
                    {
                        if (oldIntervention != null && oldIntervention.Customer != null && oldIntervention.Customer.Id == intervention.Customer.Id)
                            intervention.Customer = oldIntervention.Customer;
                        else
                            intervention.Customer = await _customerDataProvider.GetByNumberAsync(intervention.Customer.Id);
                    }
                }

                var includeCustomer = new QuerySet();
                includeCustomer.Include.Fields = new string[] { "customer" };

                // Contact can only be updated through CaseManager. Take contact of oldRequest on update.
                if (oldIntervention != null)
                    intervention.Contact = oldIntervention.Contact;
                else if (intervention.Contact != null)
                    intervention.Contact = await _contactDataProvider.GetByIdAsync(intervention.Contact.Id, includeCustomer);

                // Building can only be updated through CaseManager. Take building of oldRequest on update.
                if (oldIntervention != null)
                    intervention.Building = oldIntervention.Building;
                else if (intervention.Building != null)
                    intervention.Building = await _buildingDataProvider.GetByIdAsync(intervention.Building.Id, includeCustomer);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogDebug($"Failed to find a related entity");
                throw new IllegalArgumentException("IllegalAttribute", "Not all related entities exist.");
            }
        }

        private bool RequiresPlanningEventUpdate(Intervention existingIntervention, Intervention intervention)
        {
            return intervention.Comment != existingIntervention.Comment;
        }
    }
}