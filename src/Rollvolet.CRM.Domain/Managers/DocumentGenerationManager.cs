using System;
using System.Dynamic;
using System.IO;
using System.Linq;
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
using Rollvolet.CRM.Domain.Utils;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DocumentGenerationManager : IDocumentGenerationManager
    {
        private readonly Regex _onlyAlphaNumeric = new Regex("[^a-zA-Z0-9_]");
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly IOrderDataProvider _orderDataProvider;
        private readonly IInvoiceDataProvider _invoiceDateProvider;
        private readonly IDepositInvoiceDataProvider _depositInvoiceDateProvider;
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
        private readonly string _generatedCertificateStorageLocation;
        private readonly string _receivedCertificateStorageLocation;
        private readonly ILogger _logger;

        public DocumentGenerationManager(IRequestDataProvider requestDataProvider, IOfferDataProvider offerDataProvider,
                                         ICustomerDataProvider customerDataProvider, IContactDataProvider contactDataProvider,
                                         IOrderDataProvider orderDataProvider, IInvoiceDataProvider invoiceDateProvider,
                                         IDepositInvoiceDataProvider depositInvoiceDataProvider, ITelephoneDataProvider telephoneDataProvider,
                                         IVisitDataProvider visitDataProvider, IEmployeeDataProvider employeeDataProvider,
                                         IOptions<DocumentGenerationConfiguration> documentGenerationConfiguration,
                                         ILogger<DocumentGenerationManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _offerDataProvider = offerDataProvider;
            _orderDataProvider = orderDataProvider;
            _invoiceDateProvider = invoiceDateProvider;
            _depositInvoiceDateProvider = depositInvoiceDataProvider;
            _customerDataProvider = customerDataProvider;
            _contactDataProvider = contactDataProvider;
            _telephoneDataProvider = telephoneDataProvider;
            _visitDataProvider = visitDataProvider;
            _employeeDataProvider = employeeDataProvider;
            _httpClient = new HttpClient();
            _documentGenerationConfig = documentGenerationConfiguration.Value;
            _logger = logger;

            _offerStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.OfferStorageLocation);
            _invoiceStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.InvoiceStorageLocation);
            _productionTicketStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.ProductionTicketStorageLocation);
            _generatedCertificateStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.GeneratedCertificateStorageLocation);
            _receivedCertificateStorageLocation = FileUtils.EnsureStorageDirectory(_documentGenerationConfig.ReceivedCertificateStorageLocation);
        }

        public async Task<Stream> CreateVisitReportAsync(int requestId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "calendar-event", "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
            };
            var request = await _requestDataProvider.GetByIdAsync(requestId, query);

            await EmbedCustomerAndContactTelephonesAsync(request);

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

        public async Task<FileStream> CreateAndStoreOfferDocumentAsync(int offerId)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "offerlines", "offerlines.vat-rate", "customer", "request", "contact", "building"
            };
            var offer = await _offerDataProvider.GetByIdAsync(offerId, includeQuery);

            offer.Offerlines = offer.Offerlines.OrderBy(l => l.SequenceNumber);
            await EmbedCustomerAndContactTelephonesAsync(offer);

            string visitorInitials = null;
            if (offer.Request != null)
            {
                visitorInitials = await GetVisitorInitialsByOfferIdAsync(offer.Id);
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

        public async Task<FileStream> CreateAndStoreInvoiceDocumentAsync(int invoiceId, string language)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "contact", "building", "order", "vat-rate", "supplements", "deposits", "deposit-invoices"
            };
            var invoice = await _invoiceDateProvider.GetByIdAsync(invoiceId, includeQuery);

            string visitorInitials = null;
            if (invoice.Order != null)
            {
                var offerIncludeQuery = new QuerySet();
                offerIncludeQuery.Include.Fields = new string[] { "offerlines", "offerlines.vat-rate" };
                var offer = await _offerDataProvider.GetByIdAsync(invoice.Order.Id, offerIncludeQuery); // offer and order have the same id
                invoice.Order.Offer = offer;

                visitorInitials = await GetVisitorInitialsByOfferIdAsync(offer.Id);

                // Remove duplicated nested data before sending
                invoice.Order.Customer = null; invoice.Order.Contact = null; invoice.Order.Building = null;
                invoice.Order.Deposits = null; invoice.Order.DepositInvoices = null;
                invoice.Order.Offer.Customer = null; invoice.Order.Offer.Contact = null;
                invoice.Order.Offer.Building = null; invoice.Order.Offer.Order = null;
            }

            // Remove duplicated nested data before sending
            foreach (var deposit in invoice.Deposits) { deposit.Customer = null; deposit.Order = null; }
            foreach (var depositInvoice in invoice.DepositInvoices) { depositInvoice.Customer = null; depositInvoice.Order = null; }
            invoice.Customer.Offers = Enumerable.Empty<Offer>();
            invoice.Customer.Orders = Enumerable.Empty<Order>();

            await EmbedCustomerAndContactTelephonesAsync(invoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;
            documentData.Visitor = visitorInitials;
            documentData.language = language;

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

        public async Task<FileStream> CreateAndStoreDepositInvoiceDocumentAsync(int depositInvoiceId, string language)
        {
            var includeQuery = new QuerySet();
            includeQuery.Include.Fields = new string[] {
                "customer", "contact", "building", "order", "vat-rate"
            };
            var depositInvoice = await _depositInvoiceDateProvider.GetByIdAsync(depositInvoiceId, includeQuery);

            string visitorInitials = null;
            if (depositInvoice.Order != null)
            {
                var offerIncludeQuery = new QuerySet();
                offerIncludeQuery.Include.Fields = new string[] { "offerlines", "offerlines.vat-rate" };
                var offer = await _offerDataProvider.GetByIdAsync(depositInvoice.Order.Id, offerIncludeQuery); // offer and order have the same id
                depositInvoice.Order.Offer = offer;

                visitorInitials = await GetVisitorInitialsByOfferIdAsync(offer.Id);

                // Remove duplicated nested data before sending
                depositInvoice.Order.Customer = null; depositInvoice.Order.Contact = null; depositInvoice.Order.Building = null;
                depositInvoice.Order.Deposits = null; depositInvoice.Order.DepositInvoices = null;
                depositInvoice.Order.Offer.Customer = null; depositInvoice.Order.Offer.Contact = null;
                depositInvoice.Order.Offer.Building = null; depositInvoice.Order.Offer.Order = null;
            }

            // Remove duplicated nested data before sending
            depositInvoice.Customer.Offers = Enumerable.Empty<Offer>();
            depositInvoice.Customer.Orders = Enumerable.Empty<Order>();

            await EmbedCustomerAndContactTelephonesAsync(depositInvoice);

            dynamic documentData = new ExpandoObject();
            documentData.Invoice = depositInvoice;
            documentData.Visitor = visitorInitials;
            documentData.language = language;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/deposit-invoice";
            var filePath = ConstructInvoiceDocumentFilePath(depositInvoice);

            return await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        public async Task<FileStream> DownloadDepositInvoiceDocumentAsync(int depositInvoiceId)
        {
            var depositInvoice = await _depositInvoiceDateProvider.GetByIdAsync(depositInvoiceId);
            var filePath = ConstructInvoiceDocumentFilePath(depositInvoice);
            return DownloadDcument(filePath);
        }

        public async Task<Stream> CreateCertificateForInvoiceAsync(int invoiceId, string language)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "customer.honorific-prefix", "customer.language", "building", "contact"
            };
            var invoice = await _invoiceDateProvider.GetByIdAsync(invoiceId, query);

            return await CreateCertificateAsync(invoice, language);
        }

        public async Task UploadCertificateForInvoiceAsync(int invoiceId, Stream content)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId);
            _logger.LogDebug($"Uploading certificate to {filePath}");
            UploadDocument(filePath, content);
        }

        public async Task<FileStream> DownloadCertificateForInvoiceAsync(int invoiceId)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId);
            return DownloadDcument(filePath);
        }

        public async Task<Stream> CreateCertificateForDepositInvoiceAsync(int invoiceId, string language)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "customer", "customer.honorific-prefix", "customer.language", "building", "contact"
            };
            var depositInvoice = await _depositInvoiceDateProvider.GetByIdAsync(invoiceId, query);

            return await CreateCertificateAsync(depositInvoice, language);
        }

        public async Task UploadCertificateForDepositInvoiceAsync(int invoiceId, Stream content)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId, true);
            _logger.LogDebug($"Uploading certificate to {filePath}");
            UploadDocument(filePath, content);
        }

        public async Task<FileStream> DownloadCertificateForDepositInvoiceAsync(int invoiceId)
        {
            var filePath = await ConstructReceivedCertificateFilePathAsync(invoiceId, true);
            return DownloadDcument(filePath);
        }

        public async Task UploadProductionTicketAsync(int orderId, Stream content)
        {
            var filePath = await ConstructProductionTicketFilePathAsync(orderId);
            _logger.LogDebug($"Uploading production ticket to {filePath}");
            UploadDocument(filePath, content);
        }

        public async Task<FileStream> DownloadProductionTicketAsync(int orderId)
        {
            var filePath = await ConstructProductionTicketFilePathAsync(orderId);
            return DownloadDcument(filePath);
        }

        private async Task<Stream> CreateCertificateAsync(BaseInvoice invoice, string language)
        {
            dynamic documentData = new ExpandoObject();
            documentData.Invoice = invoice;
            documentData.language = language;

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/certificate";
            var filePath = ConstructGeneratedCertificateFilePath(invoice);

            return await GenerateAndStoreDocumentAsync(url, documentData, filePath);
        }

        private string ConstructOfferDocumentFilePath(Offer offer)
        {
            // Parse year from the offernumber, since the offerdate changes on each document generation
            // This will only work until 2099
            var year = $"20{int.Parse(offer.Number.Substring(0, 2)) - 10}";
            var directory = $"{_offerStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var number = $"{offer.Number.Substring(0, 8)}_${offer.Number.Substring(9)}"; // YY/MM/DD_nb  eg. 29/01/30_20
            var filename = _onlyAlphaNumeric.Replace($"{number  }_{offer.DocumentVersion}", "");

            return $"{directory}{filename}.pdf";
        }

        private string ConstructInvoiceDocumentFilePath(BaseInvoice invoice)
        {
            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_invoiceStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"F0{invoice.Number}", "");

            return $"{directory}{filename}.pdf";
        }

        private async Task<string> ConstructProductionTicketFilePathAsync(int orderId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] { "customer" };
            var order = await _orderDataProvider.GetByIdAsync(orderId, query);

            var year = order.OrderDate != null ? ((DateTime) order.OrderDate).Year : 0;
            var directory = $"{_productionTicketStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"{order.OfferNumber}", "") + $"_{order.Customer.Name}";

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
                invoice = await _depositInvoiceDateProvider.GetByIdAsync(invoiceId);
            else
                invoice = await _invoiceDateProvider.GetByIdAsync(invoiceId);

            var year = invoice.InvoiceDate != null ? ((DateTime) invoice.InvoiceDate).Year : 0;

            var directory = $"{_receivedCertificateStorageLocation}{year}{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            var filename = _onlyAlphaNumeric.Replace($"A0{invoice.Number}", "") + $"_{invoice.CustomerName}";

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
                    UploadDocument(filePath, inputStream);
                }

                return new FileStream(filePath, FileMode.Open);
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
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            _logger.LogDebug("Generated JSON for request body: {0}", json);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}