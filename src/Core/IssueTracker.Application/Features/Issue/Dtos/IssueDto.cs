using System;
using IssueTracker.Common.Enums;

namespace IssueTracker.Application.Features.Issue.Dtos
{
    public class IssueDto
    {
        public Guid Id { get; set; }
        public Guid Title { get; set; }
        public IssueType Type { get; set; }
        public Guid AssignedId { get; set; }
        public string TypeString => Type.ToString();
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}