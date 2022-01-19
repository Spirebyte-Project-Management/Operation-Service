using System.Threading.Tasks;
using Spirebyte.Services.Operations.Application.DTO;

namespace Spirebyte.Services.Operations.Application.Services.Interfaces;

public interface IHubService
{
    Task PublishOperationPendingAsync(OperationDto operation);
    Task PublishOperationCompletedAsync(OperationDto operation);
    Task PublishOperationRejectedAsync(OperationDto operation);
}