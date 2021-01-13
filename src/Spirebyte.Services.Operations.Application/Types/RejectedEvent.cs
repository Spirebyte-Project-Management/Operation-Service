using Convey.CQRS.Events;
using Spirebyte.Services.Operations.Application.Types.Interfaces;

namespace Spirebyte.Services.Operations.Application.Types
{
    public class RejectedEvent : IRejectedEvent, IMessage
    {
        public string Reason { get; set; }
        public string Code { get; set; }
    }
}
