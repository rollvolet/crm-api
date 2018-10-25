using System;
using System.Dynamic;
using System.IO;
using System.Net.Http;
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

namespace Rollvolet.CRM.Domain.Managers
{
    public class DocumentGenerationManager : IDocumentGenerationManager
    {
        private readonly Regex _onlyAlphaNumeric = new Regex("[^a-zA-Z0-9_]");
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDateProvider;
        private readonly ICustomerDataProvider _customerDataProvider;
        private readonly IContactDataProvider _contactDataProvider;
        private readonly ITelephoneDataProvider _telephoneDataProvider;
        private readonly IVisitDataProvider _visitDataProvider;
        private readonly IEmployeeDataProvider _employeeDataProvider;
        private readonly HttpClient _httpClient;
        private readonly DocumentGenerationConfiguration _documentGenerationConfig;
        private readonly string _offerStorageLocation;
        private readonly string _invoiceStorageLocation;
        private readonly string _productionTicketStorageLocation;
        private readonly ILogger _logger;

        public DocumentGenerationManager(IRequestDataProvider requestDataProvider, IOfferDataProvider offerDataProvider,
                                         ICustomerDataProvider customerDataProvider, IContactDataProvider contactDataProvider,
                                         IOrderDataProvider orderDataProvider, IInvoiceDataProvider invoiceDateProvider,
                                         ITelephoneDataProvider telephoneDataProvider,
                                         IVisitDataProvider visitDataProvider, IEmployeeDataProvider employeeDataProvider,
                                         IOptions<DocumentGenerationConfiguration> documentGenerationConfiguration,
                                         ILogger<DocumentGenerationManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _offerDataProvider = offerDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDateProvider = invoiceDateProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _telephoneDataProvider = telephoneDataProvider;
            _visitDataProvider = visitDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _httpClient = new HttpClient();
            _documentGenerationConfig = documentGenerationConfiguration.Value;
            _logger = logger;

            _offerStorageLocation = _documentGenerationConfig.OfferStorageLocation;
            if (!_offerStorageLocation.EndsWith(Path.DirectorySeparatorChar))
                _offerStorageLocation += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(_offerStorageLocation);

            _invoiceStorageLocation = _documentGenerationConfig.InvoiceStorageLocation;
            if (!_invoiceStorageLocation.EndsWith(Path.DirectorySeparatorChar))
                _invoiceStorageLocation += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(_invoiceStorageLocation);

            _productionTicketStorageLocation = _documentGenerationConfig.ProductionTicketStorageLocation;
            if (!_productionTicketStorageLocation.EndsWith(Path.DirectorySeparatorChar))
                _productionTicketStorageLocation += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(_productionTicketStorageLocation);
        }

        public async Task<Stream> CreateVisitReport(int requestId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "visit", "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
            };
            var request = await _requestDataProvider.GetByIdAsync(requestId, query);

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/visit-report";
            var body = GenerateJsonBody(request);
            _logger.LogDebug("Send request to document generation service at {0}", url);
            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating the visit report of request {0}: {1}", requestId, e.Message);
                throw e;
            }
        }

        public async Task<FileStream> CreateAndStoreOfferDocument(int offerId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "offerlines", "offerlines.vat-rate", "customer", "request", "contact", "building"
            };
            var offer = await _offerDataProvider.GetByIdAsync(offerId, includeQuery);

            await EmbedCustomerAndContactTelephonesAsync(offer);

            string visitorInitials = null;
            if (offer.Request != null)
            {
                try
                {
                    var visitor = await _employeeDataProvider.GetVisitorByOfferId(offer.Id);
                    visitorInitials = visitor.Initials;
                }
                catch (EntityNotFoundException)
                {
                    // No visit related to the request. Nothing must happen here.
                }
            }

            dynamic documentData = new ExpandoObject();
            documentData.Offer = offer;
            documentData.Visitor = visitorInitials;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/offer";
            var filePath = ConstructOfferDocumentFilePath(offer);

            return await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadOfferDocument(int offerId)
        {
            var offer = await _offerDataProvider.GetByIdAsync(offerId);
            var filePath = ConstructOfferDocumentFilePath(offer);
            return DownloadDcument(filePath);
        }

        public async Task<FileStream> CreateAndStoreInvoiceDocumentAsync(int invoiceId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "contact", "building", "order", "vat-rate", "supplements", "deposits", "deposit-invoices"
            };
            var invoice = await _invoiceDateProvider.GetByIdAsync(invoiceId, includeQuery);

            if (invoice.Order != null)
            {
                var orderIncludeQuery = new QuerySet();
                includeQuery.Include.Fields = new string[] { "offerlines", "offerlines.vat-rate" };
                var order = await _orderDataProvider.GetByIdAsync(invoice.Order.Id);
                invoice.Order = order;
            }

            await EmbedCustomerAndContactTelephonesAsync(invoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/invoice";
            var filePath = ConstructInvoiceDocumentFilePath(invoice);

            return await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadInvoiceDocumentAsync(int invoiceId)
        {
            var invoice = await _invoiceDateProvider.GetByIdAsync(invoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(invoice);
            return DownloadDcument(filePath);
        }

        public async Task UploadProductionTicket(int orderId, Stream content)
        {
            var filePath = await ConstructProductionTicketFilePathAsync(orderId);
            _logger.LogDebug($"Uploading production ticket to {filePath}");

            using (var fileStream = File.Create(filePath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fileStream);
            }
        }

        public async Task<FileStream> DownloadProductionTicket(int orderId)
        {
            var filePath = await ConstructProductionTicketFilePathAsync(orderId);
            return DownloadDcument(filePath);
        }

        private string ConstructOfferDocumentFilePath(Offer offer)
        {
            // Parse year from the offernumber, since the offerdate changes on each document generation
            // This will only work until 2099
            var year = $"20{int.Parse(offer.Number.Substring(0, 2)) - 10}";
            var directory = $"{_offerStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"{offer.Number}_offerte_{offer.DocumentVersion}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructInvoiceDocumentFilePath(Invoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_invoiceStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"{invoice.Number}_factuur", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> ConstructProductionTicketFilePathAsync(int orderId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, query);

            // Parse year from the offernumber, since the offerdate changes on each document generation
            // This will only work until 2099
            var year = $"20{int.Parse(order.OfferNumber.Substring(0, 2)) - 10}";
            var directory = $"{_productionTicketStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"{order.OfferNumber}_productiebon_{order.Customer.Name}".Replace(" ", "_"), "");

            return $"{directory}{filename}.pdf";
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

        private async Task<FileStream> GenerateAndStoreDocumentAsync(string url, Object data, string filePath)
        {
            var body = GenerateJsonBody(data);
            _logger.LogDebug("Send request to document generation service at {0}", url);

            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();

                using (var inputStream = await response.Content.ReadAsStreamAsync())
                {

                    using (var fileStream = File.Create(filePath))
                    {
                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.CopyTo(fileStream);
                    }
                }

                return new FileStream(filePath, FileMode.Open);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating and storing the document at {0}: {1}", filePath, e.Message);
                throw e;
            }
        }

        private FileStream DownloadDcument(string filePath)
        {
            var stream = new MemoryStream();

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

        private HttpContent GenerateJsonBody(Object data)
        {
            var json = (string) JsonConvert.SerializeObject(data, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            _logger.LogDebug("Generated JSON for request body: {0}", json);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}