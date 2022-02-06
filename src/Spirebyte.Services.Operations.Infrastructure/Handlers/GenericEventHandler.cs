using System;
using System.Threading;
using System.Threading.Tasks;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Spirebyte.Services.Operations.Application.Services.Interfaces;
using Spirebyte.Services.Operations.Core.Types;

namespace Spirebyte.Services.Operations.Infrastructure.Handlers;

public class GenericEventHandler<T> : IEventHandler<T> where T : class, IEvent, ITestEvent
{
    private readonly ICorrelationContextAccessor _contextAccessor;
    private readonly IHubService _hubService;
    private readonly IMessagePropertiesAccessor _messagePropertiesAccessor;
    private readonly IOperationsService _operationsService;

    public GenericEventHandler(ICorrelationContextAccessor contextAccessor,
        IMessagePropertiesAccessor messagePropertiesAccessor,
        IOperationsService operationsService, IHubService hubService)
    {
        _contextAccessor = contextAccessor;
        _messagePropertiesAccessor = messagePropertiesAccessor;
        _operationsService = operationsService;
        _hubService = hubService;
    }

    public async Task HandleAsync(T @event, CancellationToken cancellationToken = default)
    {
        var messageProperties = _messagePropertiesAccessor.MessageProperties;
        var correlationId = messageProperties?.CorrelationId;
        if (string.IsNullOrWhiteSpace(correlationId)) return;

        var context = _contextAccessor.GetCorrelationContext();
        var name = string.IsNullOrWhiteSpace(context?.Name) ? @event.GetType().Name : context.Name;
        var userId = context?.User?.Id;
        var state = messageProperties.GetSagaState() ?? OperationState.Completed;
        var (updated, operation) = await _operationsService.TrySetAsync(correlationId, userId, name, state, null, null,
            @event.ProjectId, @event.IssueId, @event.SprintId, @event.UserId.ToString());
        if (!updated) return;

        switch (state)
        {
            case OperationState.Pending:
                await _hubService.PublishOperationPendingAsync(operation);
                break;
            case OperationState.Completed:
                await _hubService.PublishOperationCompletedAsync(operation);
                break;
            case OperationState.Rejected:
                await _hubService.PublishOperationRejectedAsync(operation);
                break;
            default:
                throw new ArgumentException($"Invalid operation state: {state}", nameof(state));
        }
    }
}