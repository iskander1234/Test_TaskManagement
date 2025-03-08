using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Interfaces;
using FluentValidation;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskEntity>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly IValidator<GetAllTasksQuery> _validator;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository, ICacheService cacheService, IValidator<GetAllTasksQuery> validator)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
        _validator = validator;
    }

    public async Task<List<TaskEntity>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        //Валидация запроса
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Загружаем ID всех задач из БД
        var allTasks = (await _taskRepository.GetAllAsync()).ToList();

        if (!allTasks.Any()) return new List<TaskEntity>(); // Если задач нет — возвращаем пустой список

        var tasks = new List<TaskEntity>();

        foreach (var task in allTasks)
        {
            string cacheKey = $"task_{task.Id}";

            // Проверяем, есть ли конкретная задача в кэше
            var cachedTask = await _cacheService.GetAsync<TaskEntity>(cacheKey);
            if (cachedTask is not null)
            {
                tasks.Add(cachedTask);
                Console.WriteLine($"Задача {task.Id} получена из Redis.");
            }
            else
            {
                // Если задачи нет в кэше, добавляем её туда
                await _cacheService.SetAsync(cacheKey, task, TimeSpan.FromMinutes(10));
                Console.WriteLine($"Задача {task.Id} добавлена в Redis.");
                tasks.Add(task);
            }
        }

        // 🔹 Фильтруем задачи, если передан статус
        if (request.Status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == (TaskStatus)request.Status.Value).ToList();
        }

        return tasks;
    }

}