﻿using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Services.Operations;
using Spirebyte.Services.Operations.Application.DTO;
using Spirebyte.Services.Operations.Application.Services.Interfaces;

namespace Spirebyte.Services.Operations.Infrastructure.Services;

public class GrpcServiceHost : GrpcOperationsService.GrpcOperationsServiceBase
{
    private readonly ILogger<GrpcServiceHost> _logger;
    private readonly BlockingCollection<OperationDto> _operations = new();
    private readonly IOperationsService _operationsService;

    public GrpcServiceHost(IOperationsService operationsService, ILogger<GrpcServiceHost> logger)
    {
        _operationsService = operationsService;
        _logger = logger;
        _operationsService.OperationUpdated += (s, e) => _operations.TryAdd(e.Operation);
    }

    public override async Task<GetOperationResponse> GetOperation(GetOperationRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Received 'Get operation' (id: {request.Id}) request from: {context.Peer}");

        return Map(await _operationsService.GetAsync(request.Id));
    }

    public override async Task SubscribeOperations(Empty request,
        IServerStreamWriter<GetOperationResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation($"Received 'Subscribe operations' request from: {context.Peer}");
        while (true)
        {
            var operation = _operations.Take();
            await responseStream.WriteAsync(Map(operation));
        }
    }

    private static GetOperationResponse Map(OperationDto operation)
    {
        return operation is null
            ? new GetOperationResponse()
            : new GetOperationResponse
            {
                Id = operation.Id,
                UserId = operation.OwnUserId,
                Name = operation.Name,
                Code = operation.Code,
                Reason = operation.Reason,
                State = operation.State.ToString().ToLowerInvariant()
            };
    }
}