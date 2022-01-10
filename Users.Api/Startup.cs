using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Users.Api.Infrastructure.Http;
using Users.Api.ResilientPolicies;
using Users.Api.ResilientPolicies.Settings;
using Users.Api.Services;

namespace Users.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.Configure<RetryPolicySettings>(Configuration.GetSection(nameof(RetryPolicySettings)));
            services.Configure<CircuitBreakerPolicySettings>(Configuration.GetSection(nameof(CircuitBreakerPolicySettings)));

            services.AddHttpClient<IRestHttpClient, RestHttpClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44348/");
            });

            services.AddSingleton<IResilientPolicyWrapper, ResilientPolicyWrapper>();
            services.AddScoped<IAddressService, AddressService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Users.Api v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
