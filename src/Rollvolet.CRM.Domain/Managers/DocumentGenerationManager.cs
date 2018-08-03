using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rollvolet.CRM.Domain.Configuration;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.Domain.Models.Query;

namespace Rollvolet.CRM.Domain.Managers
{
    public class DocumentGenerationManager : IDocumentGenerationManager
    {
        private readonly IRequestDataProvider _requestDataProvider;
        private readonly IOfferDataProvider _offerDataProvider;
        private readonly HttpClient _httpClient;
        private readonly DocumentGenerationConfiguration _documentGenerationConfig;
        private readonly ILogger _logger;

        public DocumentGenerationManager(IRequestDataProvider requestDataProvider, IOfferDataProvider offerDataProvider,
                                         IOptions<DocumentGenerationConfiguration> documentGenerationConfiguration,                ILogger<DocumentGenerationManager> logger)
        {
            _requestDataProvider = requestDataProvider;
            _offerDataProvider = offerDataProvider;
            _httpClient = new HttpClient();
            _documentGenerationConfig = documentGenerationConfiguration.Value;
            _logger = logger;

            if (_documentGenerationConfig.OfferStorageLocation != null)
                Directory.CreateDirectory(_documentGenerationConfig.OfferStorageLocation);
        }

        public async Task<Stream> CreateVisitReport(int requestId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "visit", "customer", "customer.honorific-prefix", "customer.language", "building", "contact", "way-of-entry"
            };
            var request = await _requestDataProvider.GetByIdAsync(requestId, query);

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/visit-report";
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            _logger.LogDebug("JSON to send to document generation service at {0}: {1}", url, json);

            var body = new StringContent(json, Encoding.UTF8, "application/json");
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

        public async Task<Stream> CreateAndStoreOfferDocument(int offerId)
        {
            var query = new QuerySet();
            query.Include.Fields = new string[] {
                "offerlines", "offerlines.vat-rate", "customer", "customer.honorific-prefix", "contact", "building"
            };
            var offer = await _offerDataProvider.GetByIdAsync(offerId, query);

            var url = $"{_documentGenerationConfig.BaseUrl}/documents/offer";
            var json = JsonConvert.SerializeObject(offer, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            _logger.LogDebug("JSON to send to document generation service at {0}: {1}", url, json);

            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, body);

            try
            {
                response.EnsureSuccessStatusCode();

                var inputStream = await response.Content.ReadAsStreamAsync();
                var storageLocation = _documentGenerationConfig.OfferStorageLocation;

                if (storageLocation != null)
                {
                    if (!storageLocation.EndsWith(Path.DirectorySeparatorChar))
                        storageLocation += Path.DirectorySeparatorChar;

                    var number = offer.Number.Replace("/", "_");
                    storageLocation += $"{number}-offerte.pdf";

                    using (var fileStream = File.Create(storageLocation))
                    {
                        inputStream.Seek(0, SeekOrigin.Begin);
                        inputStream.CopyTo(fileStream);
                    }

                    inputStream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    _logger.LogInformation($"No DocumentGeneration.OfferStorageLocation configured. Not persisting offer document {offerId} to disk.");
                }

                return inputStream;
            }
            catch (Exception e)
            {
                _logger.LogWarning("Something went wrong while generating the offer document of offer {0}: {1}", offerId, e.Message);
                throw e;
            }
        }
    }
}