using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IssueTracker.Application.Interfaces;
using IssueTracker.Common.Auth;
using IssueTracker.Domain.Entities;
using IssueTracker.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IssueTracker.Persistence
{
    public static class DependencyInjection
    {

        public static IServiceProvider AddPersistence(this IServiceCollection services, IConfiguration configuration,
            string migrationsAssembly = null)
        {
            var container = new ContainerBuilder(); // create auto fac
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };
            // create db context
            services.AddDbContext<IssueTrackerDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("Default"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30.0)
                            , Array.Empty<string>());
                    });
            });
            // create identity users
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            });
            new IdentityBuilder(typeof(User), typeof(Role), services)
                .AddEntityFrameworkStores<IssueTrackerDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = "Identity.Application"; })
                .AddCookie("Identity.Application", options =>
                {
                    options.Cookie.Name = "Identity.Application";
                    options.Cookie.Domain = "Identity.Application";
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                });
            // configure identity server with in-memory stores, keys, clients and scopes
            services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "alice",
                        Password = "alice",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                                IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    }
                })
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddInMemoryApiResources(IdentityConfig.ApiResources)
                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                .AddInMemoryClients(IdentityConfig.Clients)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(configuration.GetConnectionString("Default"),
                        sql => sql.MigrationsAssembly(typeof(DependencyInjection).Namespace));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(configuration.GetConnectionString("Default"),
                        sql => sql.MigrationsAssembly(typeof(DependencyInjection).Namespace));
                });
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>>();
            services.AddScoped<RoleManager<Role>>();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder =>
            //            builder
            //                .AllowAnyOrigin()
            //                .AllowAnyMethod()
            //                .AllowAnyHeader());
            //});
            services.AddScoped<IIssueTrackerDbContext>(provider => provider.GetService<IssueTrackerDbContext>());


            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Protected API", Version = "v1"});

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize"),
                            TokenUrl = new Uri("http://localhost:5000/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"api1", "Demo API - full access"}
                            }
                        }
                    }
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        public class AuthorizeCheckOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                       .OfType<AuthorizeAttribute>().Any() ||
                                   context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

                if (hasAuthorize)
                {
                    operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
                    operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});

                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}}]
                                = new[] {"api1"}
                        }
                    };
                }
            }

        }
    }
}