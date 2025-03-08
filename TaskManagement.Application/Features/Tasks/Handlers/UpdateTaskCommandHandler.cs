using FluentValidation;
using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;
    private readonly IValidator<UpdateTaskCommand> _validator;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository, ICacheService cacheService, IValidator<UpdateTaskCommand> validator)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
        _validator = validator;
    }

    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // Валидация
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task is null) return false;

        // Обновляем данные
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = (TaskStatus)request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        // Сохраняем в БД через Dapper
        await _taskRepository.UpdateAsync(task);

        // Обновляем кэш
        await _cacheService.SetAsync($"task_{request.Id}", task, TimeSpan.FromMinutes(10));

        return true;
    }
}