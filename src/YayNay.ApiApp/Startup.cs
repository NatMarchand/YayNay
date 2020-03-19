using System;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NatMarchand.YayNay.ApiApp.Converters;
using NatMarchand.YayNay.ApiApp.Identity;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Events;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.ApproveSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.RequestSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.ScheduleSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Events;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;
using NatMarchand.YayNay.Core.Infrastructure.Events;

namespace NatMarchand.YayNay.ApiApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<RequestSessionCommandHandler>();
            services.AddTransient<ApproveSessionCommandHandler>();
            services.AddTransient<ScheduleSessionCommandHandler>();
            
            services.AddTransient<EventDispatcher>();
            services.AddTransient<IEventProcessor<SessionRequested>, ProjectSession>();
            services.AddTransient<IEventProcessor<SessionApproved>, ProjectSession>();
            services.AddTransient<IEventProcessor<SessionRejected>, ProjectSession>();
            services.AddTransient<IEventProcessor<SessionScheduled>, ProjectSession>();
            
            services.AddTransient<ISessionRepository, a>();
            services.AddTransient<IPersonProjectionStore, a>();
            services.AddTransient<ISessionProjectionStore, a>();
            services.AddTransient<SessionQueries>();

            services.AddAuthentication("MicrosoftAccount")
                .AddJwtBearer("MicrosoftAccount",
                    options =>
                    {
                        options.Authority = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0";
                        options.Audience = "7c5f96ba-3e74-4dfd-b08b-bd5869c9dc5f";
                        options.Events = new JwtBearerEvents { OnTokenValidated = OnTokenValidated };
                    });
            services.AddAuthorization();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new TypeConverterFactory());
                    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YayNay API", Version = "v1" });
                c.MapType<SessionId>(() => new OpenApiSchema { Type = "string", Format = "uuid", ReadOnly = true });
                c.MapType<PersonId>(() => new OpenApiSchema { Type = "string", Format = "uuid", ReadOnly = true });
                c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Pattern = @"^-?(\d+\.)*(\d+):(\d+):(\d+)(\.\d{0,7})?$", Example = new OpenApiString("0.12:34:56.789") });
            });

            services.AddApplicationInsightsTelemetry();

            services.AddHealthChecks();
        }

        private async Task OnTokenValidated(TokenValidatedContext arg)
        {
            var provider = arg.HttpContext.RequestServices.GetRequiredService<IPersonProjectionStore>();
            var profile = await provider.GetProfileAsync(arg.Scheme.Name, arg.Principal.FindFirstValue("sub"));
            if (profile == null)
            {
                arg.Fail("Unable to find a valid userId");
                return;
            }

            arg.Principal = new ClaimsPrincipal(YayNayIdentity.Create(profile, arg.Scheme.Name));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "YayNay API v1"); });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
                endpoints.MapGet("", context =>
                {
                    context.Response.Redirect("/swagger/index.html");
                    return Task.CompletedTask;
                });
            });
        }
    }

    public class a : ISessionProjectionStore, IPersonProjectionStore, ISessionRepository
    {
        public Task MergeProjectionAsync(SessionProjection projection)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<SessionProjection>> GetSessionsAsync(SessionStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<PersonName?> GetNameAsync(PersonId id)
        {
            throw new NotImplementedException();
        }

        public Task<PersonProfile?> GetProfileAsync(string authenticationProvider, string providerId)
        {
            throw new NotImplementedException();
        }

        public Task<Session?> GetAsync(SessionId id)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(Session session)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Session session)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Session session)
        {
            throw new NotImplementedException();
        }
    }
}
