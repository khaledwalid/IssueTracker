using System.Threading.Tasks;
using IssueTracker.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Persistence.Context
{
    public class IssueTrackerDbContext : DbContext , IIssueTrackerDbContext
    {
        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options)
            : base(options)
        {
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
