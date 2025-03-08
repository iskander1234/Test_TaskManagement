using Microsoft.AspNetCore.SignalR;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Hubs;


namespace TaskManagement.Infrastructure.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<TaskHub> _hubContext;

    public SignalRService(IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendTaskUpdate(string taskId, string title, int status)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveTaskUpdate", taskId, title, status);
    }
}