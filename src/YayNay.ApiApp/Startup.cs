using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NatMarchand.YayNay.Core.Domain.Commands.RequestSession;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Infrastructure.Events;

namespace NatMarchand.YayNay.ApiApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<RequestSessionCommandHandler>();
            services.AddTransient<EventDispatcher>();
            services.AddTransient<IEventProcessor<SessionRequested>, ProjectSessionRequested>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YayNay API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "YayNay API v1"); });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}