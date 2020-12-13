using IssueTracker.Application;
using IssueTracker.Domain.Entities;
using IssueTracker.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IssueTracker.Presentation
{
    public partial class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvcCore();
            services.AddApplication();
            services.AddPersistence(Configuration);
            services.AddControllers();
            services.AddControllersWithViews();


        }

    }
}
