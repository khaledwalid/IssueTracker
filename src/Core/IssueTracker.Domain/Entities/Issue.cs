using System;
using System.ComponentModel.DataAnnotations.Schema;
using IssueTracker.Common.Enums;

namespace IssueTracker.Domain.Entities
{
    public class Issue
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public IssueType IssueType { get; set; }
        public Guid OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User User { get; set; }
        public Status Status { get; set; }
        public Guid ProjectId { get; set; }
        [ForeignKey("Id")]
        public Project Project { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? AssignedId { get; set; }
        [ForeignKey("AssignedId")]
        public User AssignedUser { get; set; }
    }
}
