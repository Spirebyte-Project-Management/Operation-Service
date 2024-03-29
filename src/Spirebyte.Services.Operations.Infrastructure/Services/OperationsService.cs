﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Spirebyte.Services.Operations.Application.DTO;
using Spirebyte.Services.Operations.Application.Services.Args;
using Spirebyte.Services.Operations.Application.Services.Interfaces;
using Spirebyte.Services.Operations.Core.Types;
using Spirebyte.Services.Operations.Infrastructure.Configuration;

namespace Spirebyte.Services.Operations.Infrastructure.Services;

public class OperationsService : IOperationsService
{
    private readonly IDistributedCache _cache;
    private readonly RequestsOptions _options;

    public OperationsService(IDistributedCache cache, RequestsOptions options)
    {
        _cache = cache;
        _options = options;
    }

    public event EventHandler<OperationUpdatedEventArgs> OperationUpdated;

    public async Task<OperationDto> GetAsync(string id)
    {
        var operation = await _cache.GetStringAsync(GetKey(id));

        return string.IsNullOrWhiteSpace(operation) ? null : JsonConvert.DeserializeObject<OperationDto>(operation);
    }

    public async Task<(bool, OperationDto)> TrySetAsync(string id, string ownUserId, string name, OperationState state,
        string code = null, string reason = null, string projectId = null, string issueId = null,
        string sprintId = null, string userId = null)
    {
        var operation = await GetAsync(id);
        if (operation is null)
            operation = new OperationDto();
        else if (operation.State == OperationState.Completed || operation.State == OperationState.Rejected)
            return (false, operation);

        operation.Id = id;
        operation.OwnUserId = ownUserId ?? string.Empty;
        operation.Name = name;
        operation.State = state;
        operation.Code = code ?? string.Empty;
        operation.Reason = reason ?? string.Empty;
        operation.ProjectId = projectId ?? string.Empty;
        operation.IssueId = issueId ?? string.Empty;
        operation.SprintId = sprintId ?? string.Empty;
        operation.UserId = userId ?? string.Empty;
        await _cache.SetStringAsync(GetKey(id),
            JsonConvert.SerializeObject(operation),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(_options.ExpirySeconds)
            });

        OperationUpdated?.Invoke(this, new OperationUpdatedEventArgs(operation));

        return (true, operation);
    }

    private static string GetKey(string id)
    {
        return $"requests:{id}";
    }
}