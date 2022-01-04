using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Narato.Correlations;
using Narato.ExecutionTimingMiddleware;
using Rollvolet.CRM.DataProvider.Mappers;
using Rollvolet.CRM.API.Mappers;
using Rollvolet.CRM.DataProvider.Contexts;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.DataProviders;
using Rollvolet.CRM.Domain.Managers;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Rollvolet.CRM.API.Middleware.ExceptionHandling.Interfaces;
using Rollvolet.CRM.API.Middleware.ExceptionHandling;
using Rollvolet.CRM.API.Middleware.UrlRewrite;
using Microsoft.AspNetCore.Mvc;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.DataProvider.MsGraph;
using Rollvolet.CRM.Domain.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Rollvolet.CRM.DataProvider.MsGraph.Authentication;
using Rollvolet.CRM.APIContracts.JsonApi;
using Rollvolet.CRM.Business.Managers.Interfaces;
using Rollvolet.CRM.Business.Managers;
using Microsoft.Data.SqlClient;
using System.Data;
using Rollvolet.CRM.Domain.Contracts;
using Microsoft.Identity.Client;
using Rollvolet.CRM.Domain.Authentication;
using Rollvolet.CRM.Domain.Authentication.Interfaces;

namespace Rollvolet.CRM.API
{
    public class Startup
    {
        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<CrmContext>(
                options => {
                    options.UseSqlServer(Configuration["DatabaseConfiguration:ConnectionString"]);
                    // options.EnableSensitiveDataLogging(); // Remove for production
                });

            var connectionString = Configuration["DatabaseConfiguration:ConnectionString"];
            services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));

            services.Configure<ConfidentialClientApplicationOptions>(Configuration.GetSection("AzureAd"));
            services.Configure<CalendarConfiguration>(Configuration.GetSection("Calendar"));
            services.Configure<DocumentGenerationConfiguration>(Configuration.GetSection("DocumentGeneration"));
            services.Configure<FileStorageConfiguration>(Configuration.GetSection("FileStorage"));
            services.Configure<AccountancyConfiguration>(Configuration.GetSection("Accountancy"));
            services.Configure<SparqlConfiguration>(Configuration.GetSection("Sparql"));

            services.AddSession();
            services.AddCorrelations();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DataProviderAutoMapperProfileConfiguration());
                cfg.AddProfile(new DTOAutoMapperProfileConfiguration());
            });
            mapperConfiguration.AssertConfigurationIsValid();
            services.AddSingleton(sp => mapperConfiguration.CreateMapper());

            services.AddSingleton<IConfidentialClientApplicationProvider, ConfidentialClientApplicationProvider>();
            services.AddScoped<IAuthenticationProvider, OnBehalfOfMsGraphAuthenticationProvider>();

            services.AddSingleton<IExceptionToActionResultMapper, ExceptionToActionResultMapper>();
            services.AddTransient<ICustomerDataProvider, CustomerDataProvider>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<IContactDataProvider, ContactDataProvider>();
            services.AddTransient<IContactManager, ContactManager>();
            services.AddTransient<IBuildingDataProvider, BuildingDataProvider>();
            services.AddTransient<IBuildingManager, BuildingManager>();
            services.AddTransient<ITelephoneDataProvider, TelephoneDataProvider>();
            services.AddTransient<ITelephoneManager, TelephoneManager>();
            services.AddTransient<ITelephoneTypeDataProvider, TelephoneTypeDataProvider>();
            services.AddTransient<ITelephoneTypeManager, TelephoneTypeManager>();
            services.AddTransient<IRequestDataProvider, RequestDataProvider>();
            services.AddTransient<IRequestManager, RequestManager>();
            services.AddTransient<IInterventionDataProvider, InterventionDataProvider>();
            services.AddTransient<IInterventionManager, InterventionManager>();
            services.AddTransient<IOfferDataProvider, OfferDataProvider>();
            services.AddTransient<IOfferManager, OfferManager>();
            services.AddTransient<IOrderDataProvider, OrderDataProvider>();
            services.AddTransient<IOrderManager, OrderManager>();
            services.AddTransient<IInvoiceDataProvider, InvoiceDataProvider>();
            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<IDepositInvoiceDataProvider, DepositInvoiceDataProvider>();
            services.AddTransient<IDepositInvoiceManager, DepositInvoiceManager>();
            services.AddTransient<IInvoiceManager, InvoiceManager>();
            services.AddTransient<ICaseDataProvider, CaseDataProvider>();
            services.AddTransient<ICaseManager, CaseManager>();
            services.AddTransient<ITagDataProvider, TagDataProvider>();
            services.AddTransient<ITagManager, TagManager>();
            services.AddTransient<IDepositDataProvider, DepositDataProvider>();
            services.AddTransient<IDepositManager, DepositManager>();
            services.AddTransient<ICountryDataProvider, CountryDataProvider>();
            services.AddTransient<ICountryManager, CountryManager>();
            services.AddTransient<IHonorificPrefixDataProvider, HonorificPrefixDataProvider>();
            services.AddTransient<IHonorificPrefixManager, HonorificPrefixManager>();
            services.AddTransient<ILanguageDataProvider, LanguageDataProvider>();
            services.AddTransient<IProductUnitDataProvider, ProductUnitDataProvider>();
            services.AddTransient<ILanguageManager, LanguageManager>();
            services.AddTransient<IPostalCodeDataProvider, PostalCodeDataProvider>();
            services.AddTransient<IPostalCodeManager, PostalCodeManager>();
            services.AddTransient<IPaymentDataProvider, PaymentDataProvider>();
            services.AddTransient<IPaymentManager, PaymentManager>();
            services.AddTransient<IWorkingHourManager, WorkingHourManager>();
            services.AddTransient<IWorkingHourDataProvider, WorkingHourDataProvider>();
            services.AddTransient<IVatRateManager, VatRateManager>();
            services.AddTransient<IVatRateDataProvider, VatRateDataProvider>();
            services.AddTransient<IEmployeeManager, EmployeeManager>();
            services.AddTransient<IEmployeeDataProvider, EmployeeDataProvider>();
            services.AddTransient<IWayOfEntryManager, WayOfEntryManager>();
            services.AddTransient<IProductUnitManager, ProductUnitManager>();
            services.AddTransient<IWayOfEntryDataProvider, WayOfEntryDataProvider>();
            services.AddTransient<IAccountancyExportManager, AccountancyExportManager>();
            services.AddTransient<IAccountancyExportDataProvider, AccountancyExportDataProvider>();
            services.AddTransient<ICalendarEventManager, CalendarEventManager>();
            services.AddTransient<IVisitDataProvider, VisitDataProvider>();
            services.AddTransient<IInvoiceSupplementManager, InvoiceSupplementManager>();
            services.AddTransient<IInvoiceSupplementDataProvider, InvoiceSupplementDataProvider>();
            services.AddTransient<IWorkingHourManager, WorkingHourManager>();
            services.AddTransient<IWorkingHourDataProvider, WorkingHourDataProvider>();
            services.AddTransient<IPlanningEventManager, PlanningEventManager>();
            services.AddTransient<IPlanningEventDataProvider, PlanningEventDataProvider>();
            services.AddTransient<IDocumentGenerationManager, DocumentGenerationManager>();
            services.AddTransient<ISystemTaskManager, SystemTaskManager>();
            services.AddTransient<IGraphApiSystemTaskService, GraphApiFileStorageService>();
            services.AddTransient<ISystemTaskDataProvider, SystemTaskDataProvider>();
            services.AddTransient<IErrorNotificationManager, ErrorNotificationManager>();
            services.AddTransient<ISequenceDataProvider, SequenceDataProvider>();
            services.AddTransient<IJsonApiBuilder, JsonApiBuilder>();
            services.AddTransient<IIncludedCollector, IncludedCollector>();
            services.AddTransient<IReportManager, ReportManager>();
            services.AddTransient<ISystemTaskExecutor, SystemTaskExecutor>();
            services.AddTransient<IGraphApiCalendarService, GraphApiCalendarService>();

            var fileStorageLocation = Configuration["FileStorage:Location"];
            if (fileStorageLocation.ToLower() == "cloud")
                services.AddTransient<IFileStorageService, GraphApiFileStorageService>();
            else
                services.AddTransient<IFileStorageService, LocalFileStorageService>();

            services.AddControllers((opt) => {
                opt.Filters.Add(typeof(ExceptionHandlerFilter));
                opt.UseCentralRoutePrefix(new RouteAttribute("api"));
            }).AddJsonOptions((opt) => {
                opt.JsonSerializerOptions.PropertyNamingPolicy = new JsonApiNamingPolicy();
                opt.JsonSerializerOptions.Converters.Add(new IRelationshipConverter());
                opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExecutionTiming();
            }

            app.UseCorrelations();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
