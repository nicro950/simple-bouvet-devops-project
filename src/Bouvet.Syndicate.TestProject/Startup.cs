using System;
using Bouvet.Syndicate.TestProject.Configuration.Settings;
using Bouvet.Syndicate.TestProject.Configuration.StartupConfigs;
using Bouvet.Syndicate.TestProject.Helpers.BlobStorage;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Bouvet.Syndicate.TestProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // This is a change
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            services.AddApplicationInsightsTelemetry();

            services.AddMediatR(typeof(Startup));
            services.AddControllers();

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "bouvet-syndicate-test";
                document.Title = "Bouvet test web api";
            });

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "bouvet-syndicate-test-web-fix";
                document.Title = "Bouvet test web api with upload fix";
                document.DocumentProcessors.Add(new FileUploadDocumentProcessor());
            });

            services.Configure<StorageSettings>(x => Configuration.Bind("Storage", x));
            services.Configure<ServiceBusSettings>(x => Configuration.Bind("ServiceBus", x));

            services.AddSingleton<StorageManager>();

            services.AddScoped(x =>
            {
                var settings = x.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
                if (settings.ConnectionString == null || settings.QueueName == null)
                {
                    throw new NullReferenceException("Missing ServiceBus settings");
                }

                var builder = new ServiceBusConnectionStringBuilder(settings.ConnectionString);
                return new QueueClient(builder);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this 
                // for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.DocExpansion = "list";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
