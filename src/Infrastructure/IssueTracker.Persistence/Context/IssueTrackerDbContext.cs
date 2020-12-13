using System;
using System.Threading.Tasks;
using IssueTracker.Application.Interfaces;
using IssueTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Persistence.Context
{
    public class IssueTrackerDbContext : IdentityDbContext<User, Role, Guid>, IIssueTrackerDbContext
    {
        public IssueTrackerDbContext(DbContextOptions<IssueTrackerDbContext> options)
            : base(options)
        {
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DbSet<Issue> Issues { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
