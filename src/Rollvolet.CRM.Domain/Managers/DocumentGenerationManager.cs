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
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Exceptions;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models;
using Rollvolet.CRM.Domain.Models.Interfaces;
using Rollvolet.CRM.Domain.Models.Query;
using Rollvolet.CRM.Domain.Utils;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DocumentGenerationManager : IDocumentGenerationManager
    {
        private readonly Regex _onlyAlphaNumeric = new Regex("[^a-zA-Z0-9_]");
        private readonly Regex _noNewlines = new Regex("\r|\n|\r\n", RegexOptions.Multiline);
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IInterventionDataProvider _interventionDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDataProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDataProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly IBuildingDataProvider _buildingDataProvider;
        private readonly ITelephoneDataProvider _telephoneDataProvider;
        private readonly IVisitDataProvider _visitDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly HttpClient _httpClient;
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
                                         IDepositInvoiceDataProvider depositInvoiceDataProvider, ITelephoneDataProvider telephoneDataProvider,
                                         IVisitDataProvider visitDataProvider, IEmployeeDataProvider employeeDataProvider,
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
            _telephoneDataProvider = telephoneDataProvider;
            _visitDataProvider = visitDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _httpClient = new HttpClient();
            _documentGenerationConfig = documentGenerationConfiguration.Value;
            _logger = logger;

            _visitReportStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.VisitReportStorageLocation);
            _interventionReportStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.InterventionReportStorageLocation);
            _offerStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.OfferStorageLocation);
            _orderStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.OrderStorageLocation);
            _deliveryNoteStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.DeliveryNoteStorageLocation);
            _invoiceStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.InvoiceStorageLocation);
            _generatedProductionTicketStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.GeneratedProductionTicketStorageLocation);
            _receivedProductionTicketStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.ReceivedProductionTicketStorageLocation);
            _generatedCertificateStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.GeneratedCertificateStorageLocation);
            _receivedCertificateStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.ReceivedCertificateStorageLocation);
            _certificateUploadSourceLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.CertificateUploadSourceLocation);
        }

        public async Task CreateAndStoreVisitReportAsync(int requestId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "calendar-event", "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
            };
            var request = await _requestDataProvider.GetByIdAsync(requestId, query);

            await EmbedCustomerAndContactTelephonesAsync(request);

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
            var filePath = ConstructVisitReportFilePath(request);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadVisitReportAsync(int requestId)
        {
            var request = await _requestDataProvider.GetByIdAsync(requestId);
            var filePath = ConstructVisitReportFilePath(request);
            return DownloadDcument(filePath);
        }

        public async Task DeleteVisitReportAsync(int requestId)
        {
            var request = await _requestDataProvider.GetByIdAsync(requestId);
            var filePath = ConstructVisitReportFilePath(request);
            RemoveFile(filePath);
        }

        public async Task CreateAndStoreInterventionReportAsync(int interventionId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "planning-event", "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry", "employee"
            };
            var intervention = await _interventionDataProvider.GetByIdAsync(interventionId, query);

            var techniciansQuery = new QuerySet();
            techniciansQuery.Page.Size = 1000; // TODO we assume 1 intervention doesn't have more than 1000 technicians. Ideally, we should query by page.
            var technicians = await _employeeDataProvider.GetAllByInterventionIdAsync(interventionId, techniciansQuery);
            intervention.Technicians = technicians.Items.OrderBy(t => t.FirstName);

            await EmbedCustomerAndContactTelephonesAsync(intervention);

            dynamic documentData = new ExpandoObject();
            documentData.Intervention = intervention;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/intervention-report";
            var filePath = ConstructInterventionReportFilePath(intervention);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadInterventionReportAsync(int interventionId)
        {
            var intervention = await _interventionDataProvider.GetByIdAsync(interventionId);
            var filePath = ConstructInterventionReportFilePath(intervention);
            return DownloadDcument(filePath);
        }

        public async Task DeleteInterventionReportAsync(int interventionId)
        {
            var request = await _interventionDataProvider.GetByIdAsync(interventionId);
            var filePath = ConstructInterventionReportFilePath(request);
            RemoveFile(filePath);
        }

        public async Task CreateAndStoreOfferDocumentAsync(int offerId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "offerlines", "offerlines.vat-rate", "customer", "request", "contact", "building"
            };
            var offer = await _offerDataProvider.GetByIdAsync(offerId, includeQuery);

            offer.Offerlines = offer.Offerlines.OrderBy(l => l.SequenceNumber);
            await EmbedCustomerAndContactTelephonesAsync(offer);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Offer = offer;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/offer";
            var filePath = ConstructOfferDocumentFilePath(offer);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadOfferDocumentAsync(int offerId)
        {
            var offer = await _offerDataProvider.GetByIdAsync(offerId);
            var filePath = ConstructOfferDocumentFilePath(offer);
            return DownloadDcument(filePath);
        }

        public async Task DeleteOfferDocumentAsync(int offerId)
        {
            var offer = await _offerDataProvider.GetByIdAsync(offerId);
            var filePath = ConstructOfferDocumentFilePath(offer);
            RemoveFile(filePath);
        }

        public async Task CreateAndStoreOrderDocumentAsync(int orderId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] { "customer", "contact", "building", "invoicelines", "invoicelines.vat-rate" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, includeQuery);

            var offerIncludeQuery = new QuerySet();
            offerIncludeQuery.Include.Fields = new string[] { "request" };
            var offer = await _offerDataProvider.GetByOrderIdAsync(orderId, offerIncludeQuery);

            order.Invoicelines = order.Invoicelines.OrderBy(l => l.SequenceNumber);
            offer.Order = null; // Remove duplicated nested data before sending
            order.Offer = offer;

            await EmbedCustomerAndContactTelephonesAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/order";
            var filePath = ConstructOrderDocumentFilePath(order);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadOrderDocumentAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructOrderDocumentFilePath(order);
            return DownloadDcument(filePath);
        }

        public async Task DeleteOrderDocumentAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructOrderDocumentFilePath(order);
            RemoveFile(filePath);
        }

        public async Task CreateAndStoreDeliveryNoteAsync(int orderId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] { "customer", "contact", "building", "invoicelines", "invoicelines.vat-rate" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, includeQuery);

            var offerIncludeQuery = new QuerySet();
            offerIncludeQuery.Include.Fields = new string[] { "request" };
            var offer = await _offerDataProvider.GetByOrderIdAsync(orderId, offerIncludeQuery);

            order.Invoicelines = order.Invoicelines.OrderBy(l => l.SequenceNumber);
            offer.Order = null; // Remove duplicated nested data before sending
            order.Offer = offer;

            await EmbedCustomerAndContactTelephonesAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/delivery-note";
            var filePath = ConstructDeliveryNoteFilePath(order);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadDeliveryNoteAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructDeliveryNoteFilePath(order);
            return DownloadDcument(filePath);
        }

        public async Task DeleteDeliveryNoteAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructDeliveryNoteFilePath(order);
            RemoveFile(filePath);
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

            await EmbedCustomerAndContactTelephonesAsync(order);
            await EmbedBuildingTelephoneAsync(order);

            var visitorInitials = offer.Request != null ? await GetVisitorInitialsByOfferIdAsync(offer.Id) : null;

            dynamic documentData = new ExpandoObject();
            documentData.Order = order;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/production-ticket";
            var filePath = ConstructGeneratedProductionTicketFilePath(order);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadProductionTicketTemplateAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructGeneratedProductionTicketFilePath(order);
            return DownloadDcument(filePath);
        }

        public async Task DeleteProductionTicketTemplateAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);
            var filePath = ConstructGeneratedProductionTicketFilePath(order);
            RemoveFile(filePath);
        }

        public async Task UploadProductionTicketAsync(int orderId, Stream content)
        {
            var filePath = await ConstructReceivedProductionTicketFilePathAsync(orderId);
            _logger.LogDebug($"Uploading production ticket to {filePath}");
            UploadDocument(filePath, content);
        }

        public async Task<FileStream> DownloadProductionTicketAsync(int orderId)
        {
            var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
            return DownloadDcument(filePath);
        }

        public async Task DeleteProductionTicketAsync(int orderId)
        {
            try
            {
                var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
                RemoveFile(filePath);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogInformation("No production ticket found for order {0}. Nothing to delete on disk.", orderId);
            }
        }

        public async Task<Stream> DownloadProductionTicketWithWatermarkAsync(int orderId)
        {
            var filePath = await FindReceivedProductionTicketFilePathAsync(orderId);
            var file = DownloadDcument(filePath);
            return await WatermarkProductionTicketAsync(file);
        }

        private async Task<Stream> WatermarkProductionTicketAsync(FileStream productionTicketStream)
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
                "customer", "contact", "building", "order", "intervention", "vat-rate", "supplements", "supplements.unit", "deposits", "deposit-invoices", "invoicelines", "invoicelines.vat-rate"
            };
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId, includeQuery);

            invoice.Invoicelines = invoice.Invoicelines.OrderBy(l => l.SequenceNumber);

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
            foreach (var invoiceline in invoice.Invoicelines) { invoiceline.Order = null; }

            await EmbedCustomerAndContactTelephonesAsync(invoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/invoice";
            var filePath = ConstructInvoiceDocumentFilePath(invoice);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadInvoiceDocumentAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(invoice);
            return DownloadDcument(filePath);
        }

        public async Task DeleteInvoiceDocumentAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(invoice);
            RemoveFile(filePath);
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
                orderIncludeQuery.Include.Fields = new string[] { "offer", "invoicelines", "invoicelines.vat-rate" };
                var order = await _orderDataProvider.GetByIdAsync(depositInvoice.Order.Id, orderIncludeQuery);
                order.Invoicelines = order.Invoicelines.OrderBy(l => l.SequenceNumber);
                depositInvoice.Order = order;

                visitorInitials = await GetVisitorInitialsByOfferIdAsync(depositInvoice.Order.Offer.Id);

                // Remove duplicated nested data before sending
                depositInvoice.Order.Customer = null; depositInvoice.Order.Contact = null; depositInvoice.Order.Building = null;
                depositInvoice.Order.Deposits = null; depositInvoice.Order.DepositInvoices = null;
                depositInvoice.Order.Offer.Customer = null; depositInvoice.Order.Offer.Contact = null;
                depositInvoice.Order.Offer.Building = null; depositInvoice.Order.Offer.Order = null;
            }

            await EmbedCustomerAndContactTelephonesAsync(depositInvoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = depositInvoice;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/deposit-invoice";
            var filePath = ConstructInvoiceDocumentFilePath(depositInvoice);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(depositInvoice);
            return DownloadDcument(filePath);
        }

        public async Task DeleteDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(depositInvoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(depositInvoice);
            RemoveFile(filePath);
        }

        public async Task CreateCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "building", "contact"
            };
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId, query);

            await EmbedCustomerAndContactTelephonesAsync(invoice);  // required to include customer/contact language and honorific prefix

            await CreateCertificateAsync(invoice);
        }

        public async Task<FileStream> DownloadCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructGeneratedCertificateFilePath(invoice);
            return DownloadDcument(filePath);
        }

        public async Task DeleteCertificateTemplateForInvoiceAsync(int invoiceId)
        {
            var invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructGeneratedCertificateFilePath(invoice);
            RemoveFile(filePath);
        }

        public async Task UploadCertificateForInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId);
            _logger.LogDebug($"Uploading certificate to {filePath}");
            UploadDocument(filePath, content);

            RemoveFile($"{_certificateUploadSourceLocation}{uploadFileName}");
        }

        public async Task<FileStream> DownloadCertificateForInvoiceAsync(int invoiceId)
        {
            var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
            return DownloadDcument(filePath);
        }

        public async Task DeleteCertificateForInvoiceAsync(int invoiceId)
        {
            try
            {
                var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
                RemoveFile(filePath);
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

            await EmbedCustomerAndContactTelephonesAsync(depositInvoice);  // required to include customer/contact language and honorific prefix

            await CreateCertificateAsync(depositInvoice);
        }

        public async Task<FileStream> DownloadCertificateTemplateForDepositInvoiceAsync(int invoiceId)
        {
            var depositInvoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructGeneratedCertificateFilePath(depositInvoice);
            return DownloadDcument(filePath);
        }

        public async Task DeleteCertificateTemplateForDepositInvoiceAsync(int invoiceId)
        {
            var invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructGeneratedCertificateFilePath(invoice);
            RemoveFile(filePath);
        }

        public async Task UploadCertificateForDepositInvoiceAsync(int invoiceId, Stream content, string uploadFileName = null)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId, true);
            _logger.LogDebug($"Uploading certificate to {filePath}");
            UploadDocument(filePath, content);

            RemoveFile($"{_certificateUploadSourceLocation}{uploadFileName}");
        }

        public async Task<FileStream> DownloadCertificateForDepositInvoiceAsync(int invoiceId)
        {
            var filePath = await FindReceivedCertificateFilePathAsync(invoiceId, true);
            return DownloadDcument(filePath);
        }

        public async Task DeleteCertificateForDepositInvoiceAsync(int invoiceId)
        {
            try
            {
                var filePath = await FindReceivedCertificateFilePathAsync(invoiceId);
                RemoveFile(filePath);
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
            var filePath = ConstructGeneratedCertificateFilePath(invoice);

            await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        private string ConstructVisitReportFilePath(Request request)
        {
            var year = request.RequestDate != null ? ((DateTime) request.RequestDate).Year : 0;
            var directory = $"{_visitReportStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            return $"{directory}AD{request.Id}.pdf";
        }

        private string ConstructInterventionReportFilePath(Intervention intervention)
        {
            var year = intervention.Date != null ? ((DateTime) intervention.Date).Year : 0;
            var directory = $"{_interventionReportStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            return $"{directory}IR{intervention.Id}.pdf";
        }

        private string ConstructOfferDocumentFilePath(Offer offer)
        {
            // Parse year from the offernumber, since the offerdate changes on each document generation
            // This will only work until 2099
            var year = $"20{int.Parse(offer.Number.Substring(0, 2)) - 10}";
            var directory = $"{_offerStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var number = $"{offer.Number.Substring(0, 8)}_${offer.Number.Substring(9)}"; // YY/MM/DD_nb  eg. 29/01/30_20
            var filename = _onlyAlphaNumeric.Replace($"{number}_{offer.DocumentVersion}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructOrderDocumentFilePath(Order order)
        {
            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_orderStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var number = $"{order.OfferNumber.Substring(0, 8)}_${order.OfferNumber.Substring(9)}"; // YY/MM/DD_nb  eg. 29/01/30_20
            var filename = _onlyAlphaNumeric.Replace($"{number}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructDeliveryNoteFilePath(Order order)
        {
            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_deliveryNoteStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var number = $"{order.OfferNumber.Substring(0, 8)}_${order.OfferNumber.Substring(9)}"; // YY/MM/DD_nb  eg. 29/01/30_20
            var filename = _onlyAlphaNumeric.Replace($"{number}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructGeneratedProductionTicketFilePath(Order order)
        {
             var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_generatedProductionTicketStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var number = $"{order.OfferNumber.Substring(0, 8)}_${order.OfferNumber.Substring(9)}"; // YY/MM/DD_nb  eg. 29/01/30_20
            var filename = _onlyAlphaNumeric.Replace($"{number}", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> ConstructReceivedProductionTicketFilePathAsync(int orderId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, query);

            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_receivedProductionTicketStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"{order.OfferNumber}", "") + _noNewlines.Replace($"_{order.Customer.Name}", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> FindReceivedProductionTicketFilePathAsync(int orderId)
        {
            var order = await _orderDataProvider.GetByIdAsync(orderId);

            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_receivedProductionTicketStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);  // ensure directory exists

            // only search on offernumber since customer name might have changed
            var filenameSearch = _onlyAlphaNumeric.Replace($"{order.OfferNumber}", "") + "*";

            var matchingFiles = Directory.GetFiles(directory, filenameSearch);

            if (matchingFiles.Length > 0)
            {
                return matchingFiles[0];
            }
            else
            {
                _logger.LogWarning($"Cannot find production-ticket file for order {orderId} starting with '{filenameSearch}' in directory {directory}");
                throw new EntityNotFoundException();
            }
        }

        private string ConstructInvoiceDocumentFilePath(BaseInvoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_invoiceStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"F0{invoice.Number}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructGeneratedCertificateFilePath(BaseInvoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_generatedCertificateStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> ConstructReceivedCertificateFilePathAsync(int invoiceId, bool isDeposit = false)
        {
            BaseInvoice invoice = null;
            if (isDeposit)
                invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            else
                invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);

            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_receivedCertificateStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "") + _noNewlines.Replace($"_{invoice.CustomerName}", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> FindReceivedCertificateFilePathAsync(int invoiceId, bool isDeposit = false)
        {
            BaseInvoice invoice = null;
            if (isDeposit)
                invoice = await _depositInvoiceDataProvider.GetByIdAsync(invoiceId);
            else
                invoice = await _invoiceDataProvider.GetByIdAsync(invoiceId);

            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_receivedCertificateStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);  // ensure directory exists

            // only search on invoice number since customer name might have changed
            var filenameSearch = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "") + "*";

            var matchingFiles = Directory.GetFiles(directory, filenameSearch);

            if (matchingFiles.Length > 0)
            {
                return matchingFiles[0];
            }
            else
            {
                _logger.LogWarning($"Cannot find certificate file for invoice {invoiceId} starting with '{filenameSearch}' in directory {directory}");
                throw new EntityNotFoundException();
            }
        }

        private async Task EmbedCustomerAndContactTelephonesAsync(ICaseRelated resource)
        {
            var telephoneQuery = new QuerySet();
            telephoneQuery.Sort.Field = "order";
            telephoneQuery.Sort.Order = SortQuery.ORDER_ASC;
            telephoneQuery.Include.Fields = new string[] { "country", "telephone-type" };

            if (resource.Customer != null)
            {
                var customerIncludeQuery = new QuerySet();
                customerIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Customer = await _customerDataProvider.GetByNumberAsync(resource.Customer.Number, customerIncludeQuery);

                var telephones = await _telephoneDataProvider.GetAllByCustomerIdAsync(resource.Customer.Id, telephoneQuery);
                resource.Customer.Telephones = telephones.Items;
            }

            if (resource.Contact != null)
            {
                var contactIncludeQuery = new QuerySet();
                contactIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Contact = await _contactDataProvider.GetByIdAsync(resource.Contact.Id, contactIncludeQuery);

                var telephones = await _telephoneDataProvider.GetAllByContactIdAsync(resource.Contact.Id, telephoneQuery);
                resource.Contact.Telephones = telephones.Items;
            }
        }

        private async Task EmbedBuildingTelephoneAsync(ICaseRelated resource)
        {
            var telephoneQuery = new QuerySet();
            telephoneQuery.Sort.Field = "order";
            telephoneQuery.Sort.Order = SortQuery.ORDER_ASC;
            telephoneQuery.Include.Fields = new string[] { "country", "telephone-type" };

            if (resource.Building != null)
            {
                var buildingIncludeQuery = new QuerySet();
                buildingIncludeQuery.Include.Fields = new string[] { "honorific-prefix", "language" };
                resource.Building = await _buildingDataProvider.GetByIdAsync(resource.Building.Id, buildingIncludeQuery);

                var telephones = await _telephoneDataProvider.GetAllByBuildingIdAsync(resource.Building.Id, telephoneQuery);
                resource.Building.Telephones = telephones.Items;
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

        private async Task GenerateAndStoreDocumentAsync(string url, Object data, string filePath)
        {
            var body = GenerateJsonBody(data);
            _logger.LogDebug("Send request to document generation service at {0}", url);

            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();

                using (var inputStream = await response.Content.ReadAsStreamAsync())
                {
                    UploadDocument(filePath, inputStream);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating and storing the document at {0}: {1}", filePath, e.Message);
                throw e;
            }
        }

        private void UploadDocument(string filePath, Stream content)
        {
            using (var fileStream = File.Create(filePath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fileStream);
            }
        }

        private FileStream DownloadDcument(string filePath)
        {
            try
            {
                return new FileStream(filePath, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning("Cannot find document at {1}", filePath);
                throw new EntityNotFoundException();
            }
        }

        private void RemoveFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                else
                    _logger.LogDebug("File {0} not found and will not be removed.", path);
            }
            catch (Exception)
            {
                _logger.LogDebug("Failed to remove file {0}.", path);
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
    }
}