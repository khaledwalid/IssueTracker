using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IssueTracker.Application.Interfaces;
using IssueTracker.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.Logging;

namespace IssueTracker.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceProvider AddPersistence(this IServiceCollection services , IConfiguration configuration ,
            DbContextOptions dbContextOptions = null)
        {
            var container = new ContainerBuilder();
            services.AddDbContext<IssueTrackerDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("Default"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30.0)
                            , Array.Empty<string>());

                    }));
            services.AddScoped<IIssueTrackerDbContext>(provider => provider.GetService<IssueTrackerDbContext>());
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }
    }
}
