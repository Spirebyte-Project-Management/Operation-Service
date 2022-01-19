using System.Threading.Tasks;

namespace Spirebyte.Services.Operations.Application.Services.Interfaces;

public interface IHubWrapper
{
    Task PublishToUserAsync(string userId, string message, object data);
    Task PublishToProjectAsync(string projectId, string message, object data);
    Task PublishToAllAsync(string message, object data);
}