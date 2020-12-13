using IssueTracker.Application.Features.Issue.Dtos;
using IssueTracker.Common.Enums;
using MediatR;

namespace IssueTracker.Application.Features.Issue.Commands.Create
{
    public class CreateIssueCommand : IRequest<IssueDto>
    {
        public string Title { get; set; }
        public IssueType Type { get; set; }
        public string Description { get; set; }
    }
}