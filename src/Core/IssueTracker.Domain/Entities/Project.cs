using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using IssueTracker.Common.Enums;

namespace IssueTracker.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User User { get; set; }
        public Status Status { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
