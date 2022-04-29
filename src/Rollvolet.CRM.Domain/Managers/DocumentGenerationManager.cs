using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Interfaces;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DocumentGenerationManager : IDocumentGenerationManager
    {
        private readonly Regex _onlyAlphaNumeric = new Regex("[^a-zA-Z0-9_]");
        private readonly Regex _noNewlines = new Regex("\r|\n|\r\n|\t|\"|/", RegexOptions.Multiline);
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IInterventionDataProvider _interventionDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly HttpClient _httpClient;
        private readonly IFileStorageService _fileStorageService;
        private readonly DocumentGenerationConfiguration _documentGenerationConfig;
        private readonly string _visitReportStorageLocation;
        private readonly string _interventionReportStorageLocation;
        private readonly string _offerStorageLocation;
        private readonly string _orderStorageLocation;
        private readonly string _deliveryNoteStorageLocation;
        private readonly string _generatedProductionTicketStorageLocation;
        private readonly string _receivedProductionTicketStorageLocation;
        private readonly string _invoiceStorageLocation;
        private readonly string _generatedCertificateStorageLocation;
        private readonly string _receivedCertificateStorageLocation;
        private readonly string _certificateUploadSourceLocation;
        private readonly ILogger _logger;

        public DocumentGenerationManager(IRequestDataProvider requestDataProvider, IInterventionDataProvider interventionDataProvider,
                                         IOfferDataProvider offerDataProvider, ICustomerDataProvider customerDataProvider,
                                         IContactDataProvider contactDataProvider, IBuildingDataProvider buildingDataProvider,
                                         IOrderDataProvider orderDataProvider, IInvoiceDataProvider invoiceDataProvider,
                                         IDepositInvoiceDataProvider depositInvoiceDataProvider,
                                         IEmployeeDataProvider employeeDataProvider, IFileStorageService fileStorageService,
                                         IOptions<DocumentGenerationConfiguration> documentGenerationConfiguration,
                                         ILogger<DocumentGenerationManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _interventionDataProvider = interventionDataProvider;
            _offerDataProvider = offerDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDataProvider = invoiceDataProvider;
            _depositInvoiceDataProvider = depositInvoiceDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _buildingDataProvider = buildingDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _httpClient = new HttpClient();
            _fileStorageService = fileStorageService;
            _documentGenerationConfig = documentGenerationConfiguration.Value;
            _logger = logger;

            _visitReportStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.VisitReportStorageLocation);
            _interventionReportStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.InterventionReportStorageLocation);
            _offerStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.OfferStorageLocation);
            _orderStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.OrderStorageLocation);
            _deliveryNoteStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.DeliveryNoteStorageLocation);
            _invoiceStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.InvoiceStorageLocation);
            _generatedProductionTicketStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.GeneratedProductionTicketStorageLocation);
            _receivedProductionTicketStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.ReceivedProductionTicketStorageLocation);
            _generatedCertificateStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.GeneratedCertificateStorageLocation);
            _receivedCertificateStorageLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.ReceivedCertificateStorageLocation);
            _certificateUploadSourceLocation = _fileStorageService.EnsureDirectory(_documentGenerationConfig.CertificateUploadSourceLocation);
        }

        public async Task<Stream> CreateVisitSummaryAsync(IEnumerable<int> requestIds)
        {
            var requests = new List<ExpandoObject>();

            foreach (var requestId in requestIds)
            {
                var query = new QuerySet();
                query.Include.Fields = new string[] {
                    "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
                };
                var request = await _requestDataProvider.GetByIdAsync(requestId, query);

                await EmbedCustomerAndContactAsync(request);

                var offerQuery = new QuerySet();
                offerQuery.Sort.Order = SortQuery.ORDER_DESC;
                offerQuery.Sort.Field = "offer-date";
                offerQuery.Page.Size = 5;

                var pagedOffers = await _offerDataProvider.GetAllByCustomerIdAsync(request.Customer.Id, offerQuery);

                var history = new List<Object>();
                foreach (var offer in pagedOffers.Items)
                {
                    Order order = null;
                    try
                    {
                        order = await _orderDataProvider.GetByOfferIdAsync(offer.Id);
                    }
                    catch (EntityNotFoundException)
                    {
                        order = null; // No order attached to offer
                    }

                    var historicEntry = new {
                        Offer = offer,
                        Visitor = await GetVisitorInitialsByOfferIdAsync(offer.Id),
                        IsOrdered = order != null
                    };

                    // Remove nested data before sending
                    offer.Customer = null;

                    history.Add(historicEntry);
                }

                dynamic documentData = new ExpandoObject();
                documentData.Request = request;
                documentData.History = history;          

                requests.Add(documentData);      
            }

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/visit-summary";       
            return await this.GenerateDocumentAsync(url, requests);     
        }

        public async Task CreateAndStoreVisitReportAsync(int requestId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
            };
            var request = await _requestDataProvider.GetByIdAsync(requestId, query);

            await EmbedCustomerAndContactAsync(request);

            var offerQuery = new QuerySet();
            offerQuery.Sort.Order = SortQuery.ORDER_DESC;
            offerQuery.Sort.Field = "offer-date";
            offerQuery.Page.Size = 5;

            var pagedOffers = await _offerDataProvider.GetAllByCustomerIdAsync(request.Customer.Id, offerQuery);

            var history = new List<Object>();
            foreach (var offer in pagedOffers.Items)
            {
                Order order = null;
                try
                {
                    order = await _orderDataProvider.GetByOfferIdAsync(offer.Id);
                }
                catch (EntityNotFoundException)
                {
                    order = null; // No order attached to offer
                }

                var historicEntry = new {
                    Offer = offer,
                    Visitor = await GetVisitorInitialsByOfferIdAsync(offer.Id),
                    IsOrdered = order != null
                };

                // Remove nested data before sending
                offer.Customer = null;

                history.Add(historicEntry);
            }

            dynamic documentData = new ExpandoObject();
            documentData.Request = request;
            documentData.History = history;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/visit-report";
            var fileDescriptor = await ConstructVisitReportFilePathAsync(request);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadVisitReportAsync(int requestId)
        {
            var request = await _requestDataProvider.GetByIdAsync(requestId);
            var fileDescriptor = await ConstructVisitReportFilePathAsync(request);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteVisitReportAsync(int requestId)
        {
            var request = await _requestDataProvider.GetByIdAsync(requestId);
            var fileDescriptor = await ConstructVisitReportFilePathAsync(request);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreInterventionReportAsync(int interventionId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry", "employee"
            };
            var intervention = await _interventionDataProvider.GetByIdAsync(interventionId, query);

            var techniciansQuery = new QuerySet();
            techniciansQuery.Page.Size = 1000; // TODO we assume 1 intervention doesn't have more than 1000 technicians. Ideally, we should query by page.
            var technicians = await _employeeDataProvider.GetAllByInterventionIdAsync(interventionId, techniciansQuery);
            intervention.Technicians = technicians.Items.OrderBy(t => t.FirstName);

            await EmbedCustomerAndContactAsync(intervention);

            dynamic documentData = new ExpandoObject();
            documentData.Intervention = intervention;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/intervention-report";
            var fileDescriptor = await ConstructInterventionReportFilePathAsync(intervention);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadInterventionReportAsync(int interventionId)
        {
            var intervention = await _interventionDataProvider.GetByIdAsync(interventionId);
            var fileDescriptor = await ConstructInterventionReportFilePathAsync(intervention);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteInterventionReportAsync(int interventionId)
        {
            var request = await _interventionDataProvider.GetByIdAsync(interventionId);
            var fileDescriptor = await ConstructInterventionReportFilePathAsync(request);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreOfferDocumentAsync(int offerId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "request", "contact", "building"
            };
            var offer = await _offerDataProvider.GetByIdAsync(offerId, includeQuery);

            await EmbedCustomerAndContactAsync(offer);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Offer = offer;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/offer";
            var fileDescriptor = await ConstructOfferDocumentFilePathAsync(offer);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadOfferDocumentAsync(int offerId)
        {
            var offer = await _offerDataProvider.GetByIdAsync(offerId);
            var fileDescriptor = await ConstructOfferDocumentFilePathAsync(offer);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteOfferDocumentAsync(int offerId)
        {
            var offer = await _offerDataProvider.GetByIdAsync(offerId);
            var fileDescriptor = await ConstructOfferDocumentFilePathAsync(offer);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreOrderDocumentAsync(int orderId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] { "customer", "contact", "building" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, includeQuery);

            var offerIncludeQuery = new QuerySet();
            offerIncludeQuery.Include.Fields = new string[] { "request" };
            var offer = await _offerDataProvider.GetByOrderIdAsync(orderId, offerIncludeQuery);

            offer.Order = null; // Remove duplicated nested data before sending
            order.Offer = offer;

            await EmbedCustomerAndContactAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/order";
            var fileDescriptor = await ConstructOrderDocumentFilePathAsync(order);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadOrderDocumentAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructOrderDocumentFilePathAsync(order);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteOrderDocumentAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructOrderDocumentFilePathAsync(order);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreDeliveryNoteAsync(int orderId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] { "customer", "contact", "building" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, includeQuery);

            var offerIncludeQuery = new QuerySet();
            offerIncludeQuery.Include.Fields = new string[] { "request" };
            var offer = await _offerDataProvider.GetByOrderIdAsync(orderId, offerIncludeQuery);

            offer.Order = null; // Remove duplicated nested data before sending
            order.Offer = offer;

            await EmbedCustomerAndContactAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/delivery-note";
            var fileDescriptor = await ConstructDeliveryNoteFilePathAsync(order);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadDeliveryNoteAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructDeliveryNoteFilePathAsync(order);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteDeliveryNoteAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructDeliveryNoteFilePathAsync(order);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreProductionTicketTemplateAsync(int orderId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] { "customer", "contact", "building" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, includeQuery);

            var offerIncludeQuery = new QuerySet();
            offerIncludeQuery.Include.Fields = new string[] { "request" };
            var offer = await _offerDataProvider.GetByOrderIdAsync(orderId, offerIncludeQuery);

            offer.Order = null; // Remove duplicated nested data before sending

            order.Offer = offer;

            await EmbedCustomerAndContactAsync(order);
            await EmbedBuildingAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/production-ticket";
            var fileDescriptor = await ConstructGeneratedProductionTicketFilePathAsync(order);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadProductionTicketTemplateAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructGeneratedProductionTicketFilePathAsync(order);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteProductionTicketTemplateAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var fileDescriptor = await ConstructGeneratedProductionTicketFilePathAsync(order);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task UploadProductionTicketAsync(int orderId, Stream content)
        {
            var fileDescriptor = await ConstructReceivedProductionTicketFilePathAsync(orderId);
            _logger.LogDebug($"Uploading production ticket to {fileDescriptor.FilePath}");
            await _fileStorageService.UploadDocumentAsync(fileDescriptor.Parent, fileDescriptor.FileName, content);
        }

        public async Task<Stream> DownloadProductionTicketAsync(int orderId)
        {
            var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
            return await _fileStorageService.DownloadDocumentAsync(filePath);
        }

        public async Task DeleteProductionTicketAsync(int orderId)
        {
            try
            {
                var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
                await _fileStorageService.RemoveDocumentAsync(filePath);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No production ticket found for order {0}. Nothing to delete on disk.", orderId);
            }
        }

        public async Task<Stream> DownloadProductionTicketWithWatermarkAsync(int orderId)
        {
            var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
            var file = await _fileStorageService.DownloadDocumentAsync(filePath);
            return await WatermarkProductionTicketAsync(file);
        }

        private async Task<Stream> WatermarkProductionTicketAsync(Stream productionTicketStream)
        {
            var url = $"{_documentGenerationConfig.BaseUrl}/documents/production-ticket-watermark";

            using (var body = new MultipartFormDataContent())
            {
                using (var pdfContent = new StreamContent(productionTicketStream))
                {
                    pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                    body.Add(pdfContent, "file", "production-ticket.pdf");

                    var response = await _httpClient.PostAsync(url, body);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStreamAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning("Something went wrong while retrieving document from URL {0}: {1}", url, e.Message);
                        throw e;
                    }
                }
            }
        }

        public async Task CreateAndStoreInvoiceDocumentAsync(int invoiceId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "contact", "building", "order", "intervention", "vat-rate", "deposits", "deposit-invoices"
            };
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId, includeQuery);

            string visitorInitials = null;
            if (invoice.Order != null)
            {
                var offer = await _offerDataProvider.GetByIdAsync(invoice.Order.Id); // offer and order have the same id
                invoice.Order.Offer = offer;

                visitorInitials = await GetVisitorInitialsByOfferIdAsync(offer.Id);

                // Remove duplicated nested data before sending
                invoice.Order.Customer = null; invoice.Order.Contact = null; invoice.Order.Building = null;
                invoice.Order.Deposits = null; invoice.Order.DepositInvoices = null;
                invoice.Order.Offer.Customer = null; invoice.Order.Offer.Contact = null;
                invoice.Order.Offer.Building = null; invoice.Order.Offer.Order = null;
            }

            if (invoice.Intervention != null)
            {
                invoice.Intervention.Customer = null; invoice.Intervention.Contact = null; invoice.Intervention.Building = null;
            }

            // Remove duplicated nested data before sending
            foreach (var deposit in invoice.Deposits) { deposit.Customer = null; deposit.Order = null; }
            foreach (var depositInvoice in invoice.DepositInvoices) { depositInvoice.Customer = null; depositInvoice.Order = null; }

            await EmbedCustomerAndContactAsync(invoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/invoice";
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(invoice);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadInvoiceDocumentAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(invoice);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteInvoiceDocumentAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(invoice);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "contact", "building", "order", "vat-rate"
            };
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoiceId, includeQuery);

            string visitorInitials = null;
            if (depositInvoice.Order != null)
            {
                var orderIncludeQuery = new QuerySet();
                orderIncludeQuery.Include.Fields = new string[] { "offer" };
                var order = await _orderDataProvider.GetByIdAsync(depositInvoice.Order.Id, orderIncludeQuery);
                depositInvoice.Order = order;

                visitorInitials = await GetVisitorInitialsByOfferIdAsync(depositInvoice.Order.Offer.Id);

                // Remove duplicated nested data before sending
                depositInvoice.Order.Customer = null; depositInvoice.Order.Contact = null; depositInvoice.Order.Building = null;
                depositInvoice.Order.Deposits = null; depositInvoice.Order.DepositInvoices = null;
                depositInvoice.Order.Offer.Customer = null; depositInvoice.Order.Offer.Contact = null;
                depositInvoice.Order.Offer.Building = null; depositInvoice.Order.Offer.Order = null;
            }

            await EmbedCustomerAndContactAsync(depositInvoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = depositInvoice;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/deposit-invoice";
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(depositInvoice);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        public async Task<Stream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoiceId);
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(depositInvoice);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoiceId);
            var fileDescriptor = await ConstructInvoiceDocumentFilePathAsync(depositInvoice);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task CreateCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "building", "contact"
            };
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId, query);

            await EmbedCustomerAndContactAsync(invoice);  // required to include customer/contact language and honorific prefix

            await CreateCertificateAsync(invoice);
        }

        public async Task<Stream> DownloadCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructGeneratedCertificateFilePathAsync(invoice);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructGeneratedCertificateFilePathAsync(invoice);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task UploadCertificateForInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null)
        {
            var fileDescriptor = await ConstructReceivedCertificateFilePathAsync(invoiceId);
            _logger.LogDebug($"Uploading certificate to {fileDescriptor.FilePath}");
            await _fileStorageService.UploadDocumentAsync(fileDescriptor.Parent, fileDescriptor.FileName, content);
        }

        public async Task RecycleCertificateForInvoiceAsync(int invoiceId, int sourceInvoiceId, bool isDeposit)
        {
            try
            {
                var sourcePath = await FindReceivedCertificateFilePathAsync(sourceInvoiceId, isDeposit);
                var fileDescriptor = await ConstructReceivedCertificateFilePathAsync(invoiceId);
                _logger.LogInformation("Copying certificate of path {0} to path {1}", sourcePath, fileDescriptor.FilePath);
                await _fileStorageService.CopyDocumentAsync(sourcePath, fileDescriptor.Parent, fileDescriptor.FileName);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("No file found for received certificate of invoice {0}. Cannot recycle certificate for invoice {1}", sourceInvoiceId, invoiceId);
                throw new IllegalArgumentException("IllegalAttribute", "No certificate found to recycle");
            }
        }

        public async Task<Stream> DownloadCertificateForInvoiceAsync(int invoiceId)
        {
            var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
            return await _fileStorageService.DownloadDocumentAsync(filePath);
        }

        public async Task DeleteCertificateForInvoiceAsync(int invoiceId)
        {
            try
            {
                var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
                await _fileStorageService.RemoveDocumentAsync(filePath);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No file found for received certificate of invoice {0}. Nothing to delete on disk.", invoiceId);
            }
        }

        public async Task CreateCertificateTemplateForDepositInvoiceAsync(int invoiceId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "building", "contact"
            };
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId, query);

            await EmbedCustomerAndContactAsync(depositInvoice);  // required to include customer/contact language and honorific prefix

            await CreateCertificateAsync(depositInvoice);
        }

        public async Task<Stream> DownloadCertificateTemplateForDepositInvoiceAsync(int invoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructGeneratedCertificateFilePathAsync(depositInvoice);
            return await _fileStorageService.DownloadDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task DeleteCertificateTemplateForDepositInvoiceAsync(int invoiceId)
        {
            var invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            var fileDescriptor = await ConstructGeneratedCertificateFilePathAsync(invoice);
            await _fileStorageService.RemoveDocumentAsync(fileDescriptor.FilePath);
        }

        public async Task UploadCertificateForDepositInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null)
        {
            var fileDescriptor = await ConstructReceivedCertificateFilePathAsync(invoiceId, true);
            _logger.LogDebug($"Uploading certificate to {fileDescriptor.FilePath}");
            await _fileStorageService.UploadDocumentAsync(fileDescriptor.Parent, fileDescriptor.FileName, content);
        }

        public async Task RecycleCertificateForDepositInvoiceAsync(int invoiceId, int sourceInvoiceId, bool isDeposit)
        {
            try
            {
                var sourcePath = await FindReceivedCertificateFilePathAsync(sourceInvoiceId, isDeposit);
                var fileDescriptor = await ConstructReceivedCertificateFilePathAsync(invoiceId, true);
                _logger.LogInformation("Copying certificate of path {0} to path {1}", sourcePath, fileDescriptor.FilePath);
                await _fileStorageService.CopyDocumentAsync(sourcePath, fileDescriptor.Parent, fileDescriptor.FileName);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("No file found for received certificate of invoice {0}. Cannot recycle certificate for deposit invoice {1}", sourceInvoiceId, invoiceId);
                throw new IllegalArgumentException("IllegalAttribute", "No certificate found to recycle");
            }
        }

        public async Task<Stream> DownloadCertificateForDepositInvoiceAsync(int invoiceId)
        {
            var filePath = await FindReceivedCertificateFilePathAsync(invoiceId, true);
            return await _fileStorageService.DownloadDocumentAsync(filePath);
        }

        public async Task DeleteCertificateForDepositInvoiceAsync(int invoiceId)
        {
            try
            {
                var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
                await _fileStorageService.RemoveDocumentAsync(filePath);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No file found for received certificate of deposit invoice {0}. Nothing to delete on disk.", invoiceId);
            }
        }

        private async Task CreateCertificateAsync(BaseInvoice invoice)
        {
            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/certificate";
            var fileDescriptor = await ConstructGeneratedCertificateFilePathAsync(invoice);

            await GenerateAndStoreDocumentAsync(url, documentData, fileDescriptor);
        }

        private async Task<FileDescriptor> ConstructVisitReportFilePathAsync(Request request)
        {
            var year = request.RequestDate != null ? ((DateTime) request.RequestDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _visitReportStorageLocation);
            return new FileDescriptor { Parent = directory, FileName = $"AD{request.Id}.pdf" };
        }

        private async Task<FileDescriptor> ConstructInterventionReportFilePathAsync(Intervention intervention)
        {
            var year = intervention.Date != null ? ((DateTime) intervention.Date).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _interventionReportStorageLocation);
            return new FileDescriptor { Parent = directory, FileName = $"IR{intervention.Id}.pdf" };
        }

        private async Task<FileDescriptor> ConstructOfferDocumentFilePathAsync(Offer offer)
        {
            // Parse year from the offernumber, since the offerdate changes on each document generation
            // TODO This will only work until 2099
            var year = int.Parse(offer.Number.Substring(0, 2)) - 10;
            var fullYear = year < 10 ? $"200{year}" : $"20{year}";
            var directory = await _fileStorageService.CreateDirectoryAsync(fullYear.ToString(), _offerStorageLocation);
            var number = $"AD{offer.RequestNumber}";
            string filename;
            if (offer.DocumentVersion != null)
                filename = _onlyAlphaNumeric.Replace($"{number}_{offer.DocumentVersion}", "");
            else
                filename = number;
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructOrderDocumentFilePathAsync(Order order)
        {
            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _orderStorageLocation);
            var filename = $"AD{order.RequestNumber}";
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructDeliveryNoteFilePathAsync(Order order)
        {
            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _deliveryNoteStorageLocation);
            var filename = $"AD{order.RequestNumber}";
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructGeneratedProductionTicketFilePathAsync(Order order)
        {
            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _generatedProductionTicketStorageLocation);
            var filename = $"AD{order.RequestNumber}";
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructReceivedProductionTicketFilePathAsync(int orderId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, query);

            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _receivedProductionTicketStorageLocation);
            var filename = $"AD{order.RequestNumber}" + _noNewlines.Replace($"_{order.Customer.Name}", "");
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<string> FindReceivedProductionTicketFilePathAsync(int orderId)
        {
            string filePath = null;
            string notFoundWarning = null;
            if (_documentGenerationConfig.IsSearchEnabled)
            {
                var order = await _orderDataProvider.GetByIdAsync(orderId);

                var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
                var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _receivedProductionTicketStorageLocation);
                // only search on offernumber since customer name might have changed
                var filenameSearch = $"AD{order.RequestNumber}";
                filePath = await _fileStorageService.FindDocumentAsync(directory, filenameSearch);
                notFoundWarning = $"Cannot find production-ticket file for order {orderId} starting with '{filenameSearch}' in directory {directory}";
            }
            else
            {
                var fileDescriptor = await ConstructReceivedProductionTicketFilePathAsync(orderId);
                filePath = fileDescriptor.FilePath;
                notFoundWarning = $"Cannot find production-ticket file for order {orderId} at {filePath}";
            }

            if (filePath != null)
            {
                return filePath;
            }
            else
            {
                _logger.LogWarning(notFoundWarning);
                throw new EntityNotFoundException();
            }
        }

        private async Task<FileDescriptor> ConstructInvoiceDocumentFilePathAsync(BaseInvoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _invoiceStorageLocation);
            var filename = _onlyAlphaNumeric.Replace($"F0{invoice.Number}", "");
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructGeneratedCertificateFilePathAsync(BaseInvoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _generatedCertificateStorageLocation);
            var filename = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "");
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<FileDescriptor> ConstructReceivedCertificateFilePathAsync(int invoiceId, bool isDeposit = false)
        {
            BaseInvoice invoice = null;
            if (isDeposit)
                invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            else
                invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);

            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;
            var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _receivedCertificateStorageLocation);
            var filename = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "") + _noNewlines.Replace($"_{invoice.CustomerName}", "");
            return new FileDescriptor { Parent = directory, FileName = $"{filename}.pdf" };
        }

        private async Task<string> FindReceivedCertificateFilePathAsync(int invoiceId, bool isDeposit = false)
        {
            string filePath = null;
            string notFoundWarning = null;
            if (_documentGenerationConfig.IsSearchEnabled)
            {
                BaseInvoice invoice = null;
                if (isDeposit)
                    invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
                else
                    invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);

                var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;
                var directory = await _fileStorageService.CreateDirectoryAsync(year.ToString(), _receivedCertificateStorageLocation);

                // only search on invoice number since customer name might have changed
                var filenameSearch = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "");
                filePath = await _fileStorageService.FindDocumentAsync(directory, filenameSearch);
                notFoundWarning = $"Cannot find production-ticket file for invoice {invoiceId} starting with '{filenameSearch}' in directory {directory}";
            }
            else
            {
                var fileDescriptor = await ConstructReceivedCertificateFilePathAsync(invoiceId);
                filePath = fileDescriptor.FilePath;
                notFoundWarning = $"Cannot find production-ticket file for invoice {invoiceId} at {filePath}";
            }

            if (filePath != null)
            {
                return filePath;
            }
            else
            {
                _logger.LogWarning(notFoundWarning);
                throw new EntityNotFoundException();
            }
        }

        private async Task EmbedCustomerAndContactAsync(ICaseRelated resource)
        {
            if (resource.Customer != null)
            {
                var customerIncludeQuery = new QuerySet();
                customerIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Customer = await _customerDataProvider.GetByNumberAsync(resource.Customer.Number, customerIncludeQuery);
            }

            if (resource.Contact != null)
            {
                var contactIncludeQuery = new QuerySet();
                contactIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Contact = await _contactDataProvider.GetByIdAsync(resource.Contact.Id, contactIncludeQuery);
            }
        }

        private async Task EmbedBuildingAsync(ICaseRelated resource)
        {
            if (resource.Building != null)
            {
                var buildingIncludeQuery = new QuerySet();
                buildingIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Building = await _buildingDataProvider.GetByIdAsync(resource.Building.Id, buildingIncludeQuery);
            }
        }

        private async Task<string> GetVisitorInitialsByOfferIdAsync(int offerId)
        {
            try
            {
                var visitor = await _employeeDataProvider.GetVisitorByOfferIdAsync(offerId);
                return visitor.Initials;
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        private async Task<Stream> GenerateDocumentAsync(string url, Object data)
        {
            var body = GenerateJsonBody(data);
            _logger.LogDebug("Send request to document generation service at {0}", url);

            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating document: {1}", e.Message);
                throw e;
            }
        }

        private async Task GenerateAndStoreDocumentAsync(string url, Object data, FileDescriptor fileDescriptor)
        {
            var body = GenerateJsonBody(data);
            _logger.LogDebug("Send request to document generation service at {0}", url);

            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();

                using (var inputStream = await response.Content.ReadAsStreamAsync())
                {
                    await _fileStorageService.UploadDocumentAsync(fileDescriptor.Parent, fileDescriptor.FileName, inputStream);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating and storing the document at {0}: {1}", fileDescriptor.FilePath, e.Message);
                throw e;
            }
        }

        private HttpContent GenerateJsonBody(Object data)
        {
            var json = (string) JsonConvert.SerializeObject(data, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            // Replace Newtonsoft.JSON with System.Text.Json once circular references can be ignored
            // See https://github.com/dotnet/runtime/issues/30820
            // var json = JsonSerializer.Serialize(data, new JsonSerializerOptions {
            //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            // });

            _logger.LogDebug("Generated JSON for request body: {0}", json);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public class FileDescriptor
        {
            public string Parent { get; set; }
            public string FileName { get; set; }

            public string FilePath
            {
                get
                {
                    return $"{Parent}{Path.DirectorySeparatorChar}{FileName}";
                }
            }
        }
    }
}