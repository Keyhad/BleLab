﻿using Autofac;
using Autofac.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MeshSimRestApi.Filters;
using MeshSimRestApi.Modules;
using System.IO;

/*
 * This project have been originally created from ASP.Net Core RESTful Service Template.
 * Getting started guide: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki/Getting-Started-Guide
 * More information about configuring project: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki
 */

namespace MeshSimRestApi
{
    public class Startup
    {
        ILogger<Startup> Logger { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Logger = logger;
            Startup.Configuration = configuration;

            // https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#using-environment-variables-in-configuration-options
            var envPath = Path.Combine(env.ContentRootPath, ".env");
            if (File.Exists(envPath))
            {
                DotNetEnv.Env.Load();
            }

            // See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#content-formatting
            JsonConvert.DefaultSettings = () =>
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
#if DEBUG
                    Formatting = Formatting.Indented
#else
                    Formatting = Formatting.None
#endif
                };
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors()
                // Add useful interface for accessing the ActionContext outside a controller.
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                // Add useful interface for accessing the HttpContext outside a controller.
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                // Add useful interface for accessing the IUrlHelper outside a controller.
                .AddScoped<IUrlHelper>(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
                .AddMvcCore(options =>
                {
                    options.Filters.Add(new ValidateModelFilter()); // Validate model. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#model-validation
                    options.Filters.Add(new CacheControlFilter());  // Add "Cache-Control" header. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#cache-control
                })
                .AddApiExplorer()
                .AddJsonFormatters()    // See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#content-formatting
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
#if DEBUG
                    options.SerializerSettings.Formatting = Formatting.Indented;
#else
                    options.SerializerSettings.Formatting = Formatting.None;
#endif
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddAutoMapper()    // Check out Configuration/AutoMapperProfiles/DefaultProfile to do actual configuration. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#automapper
                .AddSwagger();      // Check out Configuration/DependenciesConfig.cs/AddSwagger to do actual configuration. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#documenting-api
        }

        /// <summary>
        /// Configure Autofac DI-container
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <remarks>
        /// ConfigureContainer is where you can register things directly
        /// with Autofac. This runs after ConfigureServices so the things
        /// here will override registrations made in ConfigureServices.
        /// Don't build the container; that gets done for you.
        /// 
        /// See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#dependency-injection
        /// </remarks>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add things to the Autofac ContainerBuilder.
            builder.RegisterModule<DefaultModule>();
            builder.RegisterModule(new ConfigurationModule(Configuration));
        }

        /// <summary>
        /// Configure Autofac DI-container for production
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <remarks>
        /// This only gets called if your environment is Production. The
        /// default ConfigureContainer won't be automatically called if this
        /// one is called.
        /// 
        /// See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#dependency-injection
        /// </remarks>
        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            ConfigureContainer(builder);

            // Add things to the ContainerBuilder that are only for the
            // production environment.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app/*, IHostingEnvironment env*/)
        {
            // Use an exception handler middleware before any other handlers
            // See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#unhandled-exceptions-handling
            app.UseExceptionHandler();

            // See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#cross-origin-resource-sharing-cors-and-preflight-requests
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app
                .UseOptionsVerbHandler()    // Options verb handler must be added after CORS. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#cross-origin-resource-sharing-cors-and-preflight-requests
                .UseSwaggerWithOptions();   // Check out Configuration/MiddlewareConfig.cs/UseSwaggerWithOptions to do actual configuration. See: https://github.com/drwatson1/AspNet-Core-REST-Service/wiki#documenting-api

            app.UseMvcWithDefaultRoute();

            Logger.LogInformation("Server started");
        }
    }
}
