using System.Threading.Tasks;

namespace IssueTracker.Application.Interfaces
{
    public interface IIssueTrackerDbContext
    {
        Task<int> SaveChangesAsync();
    }
}
