using Convey.CQRS.Events;
using Spirebyte.Services.Operations.Application.Types.Interfaces;
using System;

namespace Spirebyte.Services.Operations.Infrastructure
{
    public interface ITestEvent : IEvent
    {
        string ProjectId { get; set; }
        string IssueId { get; set; }
        string SprintId { get; set; }
        Guid UserId { get; set; }
    }

    public class TestEvent : ITestEvent, IMessage
    {
        public string ProjectId { get; set; }
        public string IssueId { get; set; }
        public string SprintId { get; set; }
        public Guid UserId { get; set; }
    }
}
