using Microsoft.Extensions.Logging;
using MassTransit;

namespace TaskManagement.Application.Evets;

public class TaskCreatedEventHandler : IConsumer<TaskCreatedEvent>
{
    private readonly ILogger<TaskCreatedEventHandler> _logger;

    public TaskCreatedEventHandler(ILogger<TaskCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TaskCreatedEvent> context)
    {
        Console.WriteLine($"Получено событие: {context.Message.TaskId} - {context.Message.Title}");
        return Task.CompletedTask;
    }
}