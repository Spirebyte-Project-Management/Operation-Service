using Microsoft.AspNetCore.SignalR;
using Spirebyte.Services.Operations.Application.Services.Interfaces;
using Spirebyte.Services.Operations.Infrastructure.Hubs;
using System.Threading.Tasks;

namespace Spirebyte.Services.Operations.Infrastructure.Services
{
    public class HubWrapper : IHubWrapper
    {
        private readonly IHubContext<SpirebyteHub> _hubContext;

        public HubWrapper(IHubContext<SpirebyteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishToUserAsync(string userId, string message, object data)
            => await _hubContext.Clients.Group(userId.ToUserGroup()).SendAsync(message, data);

        public async Task PublishToProjectAsync(string projectId, string message, object data)
            => await _hubContext.Clients.Group(projectId.ToProjectGroup()).SendAsync(message, data);

        public async Task PublishToAllAsync(string message, object data)
            => await _hubContext.Clients.All.SendAsync(message, data);
    }
}
