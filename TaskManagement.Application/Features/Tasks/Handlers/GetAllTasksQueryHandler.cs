using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Interfaces;
using FluentValidation;

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
        // Валидация
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string cacheKey = "tasks";

        // Проверяем, есть ли данные в кэше
        var cachedTasks = await _cacheService.GetAsync<List<TaskEntity>>(cacheKey);
        if (cachedTasks is not null)
        {
            Console.WriteLine("Данные получены из кэша!");
            return cachedTasks;
        }

        // Загружаем из БД через Dapper
        var tasks = (await _taskRepository.GetAllAsync()).ToList();

        // Кэшируем результат
        await _cacheService.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(10));
        Console.WriteLine("Данные сохранены в Redis!");

        return tasks;
    }
}