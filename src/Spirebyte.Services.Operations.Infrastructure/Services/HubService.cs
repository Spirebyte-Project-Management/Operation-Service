using System.Threading.Tasks;
using Spirebyte.Services.Operations.Application.DTO;
using Spirebyte.Services.Operations.Application.Services.Interfaces;

namespace Spirebyte.Services.Operations.Infrastructure.Services;

public class HubService : IHubService
{
    private readonly IHubWrapper _hubContextWrapper;

    public HubService(IHubWrapper hubContextWrapper)
    {
        _hubContextWrapper = hubContextWrapper;
    }

    public async Task PublishOperationPendingAsync(OperationDto operation)
    {
        await _hubContextWrapper.PublishToAllAsync("operation_pending",
            new
            {
                id = operation.Id,
                name = operation.Name,
                sentBy = operation.OwnUserId,
                projectId = operation.ProjectId,
                issueId = operation.IssueId,
                sprintId = operation.SprintId,
                userId = operation.UserId
            }
        );
    }

    public async Task PublishOperationCompletedAsync(OperationDto operation)
    {
        await _hubContextWrapper.PublishToAllAsync("operation_completed",
            new
            {
                id = operation.Id,
                name = operation.Name,
                sentBy = operation.OwnUserId,
                projectId = operation.ProjectId,
                issueId = operation.IssueId,
                sprintId = operation.SprintId,
                userId = operation.UserId
            }
        );
    }

    public async Task PublishOperationRejectedAsync(OperationDto operation)
    {
        await _hubContextWrapper.PublishToAllAsync("operation_rejected",
            new
            {
                id = operation.Id,
                name = operation.Name,
                sentBy = operation.OwnUserId,
                code = operation.Code,
                reason = operation.Reason,
                projectId = operation.ProjectId,
                issueId = operation.IssueId,
                sprintId = operation.SprintId,
                userId = operation.UserId
            }
        );
    }
}