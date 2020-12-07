using System.Threading.Tasks;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Application.Interfaces
{
    public interface IIssueTrackerDbContext
    {
        Task<int> SaveChangesAsync();
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
    }
}
