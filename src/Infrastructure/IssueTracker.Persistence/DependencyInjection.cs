using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IssueTracker.Application.Interfaces;
using IssueTracker.Common.Auth;
using IssueTracker.Domain.Entities;
using IssueTracker.Persistence.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.Persistence
{
    public static class DependencyInjection
    {

        public static IServiceProvider AddPersistence(this IServiceCollection services, IConfiguration configuration,
            string migrationsAssembly = null)
        {
            var container = new ContainerBuilder(); // create auto fac
            // create db context
            services.AddDbContext<IssueTrackerDbContext>(options =>
            {
                //options.UseInMemoryDatabase("Memory");
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

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });
            // create identity server
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.ApiName = "api1";
                    options.Authority = "https://localhost:5000";
                });

            // configure identity server with in-memory stores, keys, clients and scopes
            services
                .AddIdentityServer()
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddInMemoryApiResources(Configuration.GetApis())
                .AddInMemoryClients(Configuration.GetClients())
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

            services.AddScoped<IIssueTrackerDbContext>(provider => provider.GetService<IssueTrackerDbContext>());
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}
