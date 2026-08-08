using HPParkingAPI.Models.DTOs.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace HPParkingAPI.Hubs;

public class GateAccessHub : Hub
{
    public async Task JoinSiteGroup(string siteId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"site_{siteId}");
    }

    public async Task LeaveSiteGroup(string siteId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"site_{siteId}");
    }

    public async Task BroadcastGateEvent(GateAccessEventDto evt)
    {
        await Clients.Group($"site_{evt.SiteId}").SendAsync("ReceiveGateAccessEvent", evt);
    }
}
