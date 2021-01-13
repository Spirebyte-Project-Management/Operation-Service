using Spirebyte.Services.Operations.Application.DTO;
using Spirebyte.Services.Operations.Application.Services.Args;
using Spirebyte.Services.Operations.Core.Types;
using System;
using System.Threading.Tasks;

namespace Spirebyte.Services.Operations.Application.Services.Interfaces
{
    public interface IOperationsService
    {
        event EventHandler<OperationUpdatedEventArgs> OperationUpdated;
        Task<OperationDto> GetAsync(string id);

        Task<(bool updated, OperationDto operation)> TrySetAsync(string id, string ownUserId, string name,
            OperationState state, string code = null, string reason = null, string projectId = null, string issueId = null, string sprintId = null, string userId = null);
    }
}
