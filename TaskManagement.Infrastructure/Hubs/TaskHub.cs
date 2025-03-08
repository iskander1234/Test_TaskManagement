using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.Infrastructure.Hubs;

public class TaskHub : Hub
{
    public async Task SendTaskUpdate(string taskId, string title, int status)
    {
        await Clients.All.SendAsync("ReceiveTaskUpdate", taskId, title, status);
    }
}