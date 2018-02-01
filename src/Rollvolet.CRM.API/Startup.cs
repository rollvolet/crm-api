using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Narato.Correlations;
using Narato.ExecutionTimingMiddleware;
using Narato.ResponseMiddleware;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;
using Rollvolet.CRM.DataProvider.Mappers;
using Swashbuckle.Swagger.Model;
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
                    jwtOptions.Audience = Configuration["Authentication:ClientId"];
                    jwtOptions.Authority = Configuration["Authentication:Authority"];
                    jwtOptions.Events = new JwtBearerEvents
                    {
                        // OnAuthenticationFailed = AuthenticationFailed
                    };
                }
            );

            services.AddMvc();

            services.AddSingleton(sp => _mapperConfiguration.CreateMapper());

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

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Contact = new Swashbuckle.Swagger.Model.Contact { Name = "MOOF bvba" },
                    Description = "Rollvolet CRM API",
                    Version = "v1",
                    Title = "Rollvolet.CRM.API"
                });

                var xmlPaths = GetXmlCommentsPaths();
                foreach (var entry in xmlPaths)
                {
                    try
                    {
                        options.IncludeXmlComments(entry);
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            services.AddCorrelations();
            services.AddResponseMiddleware();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // auto migrations
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<CrmContext>().Database.Migrate();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            app.UseCorrelations();
            app.UseExecutionTiming();

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();

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
