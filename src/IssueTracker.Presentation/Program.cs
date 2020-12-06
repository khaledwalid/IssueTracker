using System;
using System.IO;
using System.Reflection;
using IssueTracker.Persistence.Context;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IssueTracker.WebHostExtensions;

namespace IssueTracker.Presentation
{
    public class Program
    {
        public static IConfiguration Configuration => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args)
                .Build()
                .MigrateDbContext<IssueTrackerDbContext>((context, services) => { })
                .Run();

        }





        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHosting = WebHost.CreateDefaultBuilder(args)
                .UseSetting("https_port", "443");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Stage" ||
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                webHosting = webHosting.UseIIS();
            else
                webHosting = webHosting.UseKestrel();

            return webHosting
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var configurationBuilder = new ConfigurationBuilder();
                    var env = builderContext.HostingEnvironment;

                    configurationBuilder.AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    config.AddConfiguration(configurationBuilder.Build());
                });
        }
    }
}
namespace IssueTracker.WebHostExtensions
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();

                    seeder(context, services);

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}