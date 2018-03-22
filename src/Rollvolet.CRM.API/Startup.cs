using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Narato.Correlations;
using Narato.ExecutionTimingMiddleware;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;
using Rollvolet.CRM.DataProvider.Mappers;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rollvolet.CRM.API.Mappers;
using Rollvolet.CRM.DataProvider.Contexts;
using Microsoft.EntityFrameworkCore;
using Rollvolet.CRM.Domain.Contracts.DataProviders;
using Rollvolet.CRM.DataProviders;
using Rollvolet.CRM.Domain.Managers;
using Rollvolet.CRM.Domain.Managers.Interfaces;
using System.Linq;
using Rollvolet.CRM.DataProvider.Models;
using Rollvolet.CRM.API.Builders;
using Rollvolet.CRM.API.Builders.Interfaces;
using Rollvolet.CRM.API.Collectors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Rollvolet.CRM.API.Configuration;
using Rollvolet.CRM.API.Middleware.ExceptionHandling.Interfaces;
using Rollvolet.CRM.API.Middleware.ExceptionHandling;
using Rollvolet.CRM.Domain.Logging;

namespace Rollvolet.CRM.API
{
    public class Startup
    {
        private MapperConfiguration _mapperConfiguration { get; set; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("config.json")
                .AddJsonFile("config.json.local", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            env.ConfigureNLog("nlog.config");

            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DataProviderAutoMapperProfileConfiguration());
                cfg.AddProfile(new DTOAutoMapperProfileConfiguration());
            });
            _mapperConfiguration.AssertConfigurationIsValid();
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
            services.AddAuthentication(
                options => {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(
                jwtOptions => {
                    jwtOptions.Audience = Configuration["Authentication:ClientId"]; // aud value in JWT token
                    jwtOptions.Authority = $"{Configuration["Authentication:BaseUri"]}/{Configuration["Authentication:TenantId"]}/v2.0"; // iss value in JWT token
                    jwtOptions.Events = new JwtBearerEvents
                    {
                        // OnAuthenticationFailed = AuthenticationFailed
                    };
                }
            );

            services.AddCorrelations();

            services.AddSingleton(sp => _mapperConfiguration.CreateMapper());

            services.AddTransient<IExceptionToActionResultMapper, ExceptionToActionResultMapper>();

            services.AddTransient<ICustomerDataProvider, CustomerDataProvider>();
            services.AddTransient<ICustomerManager, CustomerManager>();
            services.AddTransient<IContactDataProvider, ContactDataProvider>();
            services.AddTransient<IContactManager, ContactManager>();
            services.AddTransient<IBuildingDataProvider, BuildingDataProvider>();
            services.AddTransient<IBuildingManager, BuildingManager>();
            services.AddTransient<ITelephoneDataProvider, TelephoneDataProvider>();
            services.AddTransient<ITelephoneManager, TelephoneManager>();
            services.AddTransient<IRequestDataProvider, RequestDataProvider>();
            services.AddTransient<IRequestManager, RequestManager>();
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
            services.AddTransient<IHonorificPrefixDataProvider, HonorificPrefixDataProvider>();
            services.AddTransient<ILanguageDataProvider, LanguageDataProvider>();
            services.AddTransient<IPostalCodeDataProvider, PostalCodeDataProvider>();
            services.AddTransient<ISequenceDataProvider, SequenceDataProvider>();
            services.AddTransient<IJsonApiBuilder, JsonApiBuilder>();
            services.AddTransient<IIncludedCollector, IncludedCollector>();

            services.AddMvc((config) => {
                config.Filters.Add(typeof(ExceptionHandlerFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // auto migrations
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<CrmContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                app.UseExecutionTiming();
            }
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            ApplicationLogging.LoggerFactory = loggerFactory;

            app.UseCorrelations();

            app.UseAuthentication();

            app.UseMvc();
        }

        private List<string> GetXmlCommentsPaths()
        {
            var app = PlatformServices.Default.Application;
            var files = new List<string>()
                        {
                            "Rollvolet.CRM.API.xml"
                        };

            List<string> paths = new List<string>();
            foreach (var file in files)
            {
                paths.Add(Path.Combine(app.ApplicationBasePath, file));
            }

            return paths;
        }
    }
}
