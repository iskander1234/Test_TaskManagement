using AutoMapper;
using FluentValidation;
using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly IValidator<CreateTaskCommand> _validator;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        ICacheService cacheService,
        IValidator<CreateTaskCommand> validator,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Валидация
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Проверяем дубликат Title
        var allTasks = await _taskRepository.GetAllAsync();
        if (allTasks.Any(t => t.Title == request.Title))
        {
            throw new Exception("Task with this title already exists.");
        }

        // Маппим команду в сущность
        var task = _mapper.Map<TaskEntity>(request);
        task.Id = Guid.NewGuid();
        task.Status = TaskStatus.ToDo;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        // Сохраняем в БД через Dapper
        await _taskRepository.CreateAsync(task);

        // Обновляем кэш
        await _cacheService.RemoveAsync("tasks");
        Console.WriteLine("Кэш очищен после добавления новой задачи!");

        return task.Id;
    }
}
