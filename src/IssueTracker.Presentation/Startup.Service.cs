using IssueTracker.Application;
using IssueTracker.Persistence;
using Microsoft.Extensions.DependencyInjection;

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


        }

    }
}
