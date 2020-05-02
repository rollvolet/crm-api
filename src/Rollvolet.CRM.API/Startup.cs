﻿using AutoMapper;
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
using Rollvolet.CRM.API.Configuration;
using Rollvolet.CRM.Domain.Contracts.MsGraph;
using Rollvolet.CRM.DataProvider.MsGraph;
using Rollvolet.CRM.Domain.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Graph;
using Rollvolet.CRM.DataProvider.MsGraph.Authentication;

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

            services.Configure<AuthenticationConfiguration>(Configuration.GetSection("Authentication"));
            services.Configure<CalendarConfiguration>(Configuration.GetSection("Calendar"));
            services.Configure<DocumentGenerationConfiguration>(Configuration.GetSection("DocumentGeneration"));
            services.Configure<AccountancyConfiguration>(Configuration.GetSection("Accountancy"));

/*             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                jwtOptions => {
                    AuthenticationConfiguration authConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfiguration>();
                    jwtOptions.Audience = authConfig.ClientId; // aud value in JWT token
                    jwtOptions.Authority = authConfig.Authority; // iss value in JWT token
                    jwtOptions.Events = new JwtBearerEvents {};
                    jwtOptions.SaveToken = true; // token gets saved in AuthenticationProperties on the request
                }
            ); */

            services.AddSignIn(Configuration)
                    .AddProtectedWebApi(Configuration)
                    .AddWebAppCallsProtectedWebApi(Configuration, new string[] { "User.Read", "Calendars.ReadWrite.Shared" })
                    .AddInMemoryTokenCaches();
            services.AddTransient<IAuthenticationProvider, OnBehalfOfMsGraphAuthenticationProvider>();

            services.AddSession();
            services.AddCorrelations();

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DataProviderAutoMapperProfileConfiguration());
                cfg.AddProfile(new DTOAutoMapperProfileConfiguration());
            });
            mapperConfiguration.AssertConfigurationIsValid();
            services.AddSingleton(sp => mapperConfiguration.CreateMapper());

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                return settings;
            };

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
            services.AddTransient<IOfferlineDataProvider, OfferlineDataProvider>();
            services.AddTransient<IOfferlineManager, OfferlineManager>();
            services.AddTransient<IOrderDataProvider, OrderDataProvider>();
            services.AddTransient<IOrderManager, OrderManager>();
            services.AddTransient<IInvoicelineDataProvider, InvoicelineDataProvider>();
            services.AddTransient<IInvoicelineManager, InvoicelineManager>();
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
            services.AddTransient<ISystemTaskDataProvider, SystemTaskDataProvider>();
            services.AddTransient<IErrorNotificationManager, ErrorNotificationManager>();
            services.AddTransient<IGraphApiService, GraphApiService>();
            services.AddTransient<ISequenceDataProvider, SequenceDataProvider>();
            services.AddTransient<IJsonApiBuilder, JsonApiBuilder>();
            services.AddTransient<IIncludedCollector, IncludedCollector>();

            services.AddControllers((opt) => {
                opt.Filters.Add(typeof(ExceptionHandlerFilter));
                opt.UseCentralRoutePrefix(new RouteAttribute("api"));
            }).AddNewtonsoftJson((jsonOpt) => {
                // TODO convert to new JSON in .Net Core 3.1
                jsonOpt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // auto migrations
            // using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            // {
            //     scope.ServiceProvider.GetRequiredService<CrmContext>().Database.Migrate();
            // }

            if (env.IsDevelopment())
            {
                app.UseExecutionTiming();
            }

            app.UseCorrelations();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
