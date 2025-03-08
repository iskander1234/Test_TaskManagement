using Microsoft.Extensions.Logging;
using MassTransit;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Evets;

public class TaskCreatedEventHandler : IConsumer<TaskCreatedEvent>
{
    private readonly ISignalRService _signalRService;
    private readonly ILogger<TaskCreatedEventHandler> _logger;

    public TaskCreatedEventHandler(ISignalRService signalRService, ILogger<TaskCreatedEventHandler> logger)
    {
        _signalRService = signalRService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TaskCreatedEvent> context)
    {
        _logger.LogInformation($"Новая задача создана: {context.Message.TaskId} - {context.Message.Title}");

        // Уведомляем всех клиентов через SignalR
        await _signalRService.SendTaskUpdate(context.Message.TaskId.ToString(), context.Message.Title, 0);
    }
}