using Convey.CQRS.Queries;
using Spirebyte.Services.Operations.Application.DTO;

namespace Spirebyte.Services.Operations.Application.Queries
{
    public class GetOperation : IQuery<OperationDto>
    {
        public string OperationId { get; set; }
    }
}
