
using Microsoft.Extensions.Configuration;

namespace IssueTracker.Presentation
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


    }
}
