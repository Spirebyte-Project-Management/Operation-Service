using System;
using System.Threading.Tasks;
using Convey.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Spirebyte.Services.Operations.Infrastructure.Hubs;

public class SpirebyteHub : Hub
{
    private readonly IJwtHandler _jwtHandler;

    public SpirebyteHub(IJwtHandler jwtHandler)
    {
        _jwtHandler = jwtHandler;
    }

    public async Task InitializeAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) await DisconnectAsync();
        try
        {
            var payload = _jwtHandler.GetTokenPayload(token);
            if (payload is null)
            {
                await DisconnectAsync();

                return;
            }

            var group = Guid.Parse(payload.Subject).ToUserGroup();
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await ConnectAsync();
        }
        catch
        {
            await DisconnectAsync();
        }
    }

    private async Task ConnectAsync()
    {
        await Clients.Client(Context.ConnectionId).SendAsync("connected");
    }

    private async Task DisconnectAsync()
    {
        await Clients.Client(Context.ConnectionId).SendAsync("disconnected");
    }
}