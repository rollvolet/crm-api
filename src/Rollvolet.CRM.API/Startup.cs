﻿using AutoMapper;
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
            services.AddSingleton(sp => _mapperConfiguration.CreateMapper());

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Contact = new Contact { Name = "MOOF bvba" },
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
                    catch (Exception e)
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
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            app.UseCorrelations();
            app.UseExecutionTiming();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
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
