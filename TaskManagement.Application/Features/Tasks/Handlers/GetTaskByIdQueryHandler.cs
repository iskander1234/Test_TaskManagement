using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskEntity?>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
    }

    public async Task<TaskEntity?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"task_{request.Id}";

        // Проверяем кэш
        var cachedTask = await _cacheService.GetAsync<TaskEntity>(cacheKey);
        if (cachedTask is not null)
        {
            Console.WriteLine($"Задача {request.Id} получена из кэша!");
            return cachedTask;
        }

        // Загружаем из БД
        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task is null) return null;

        // Кэшируем результат
        await _cacheService.SetAsync(cacheKey, task, TimeSpan.FromMinutes(10));
        Console.WriteLine($"Задача {request.Id} сохранена в Redis!");

        return task;
    }
}