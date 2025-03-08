using FluentValidation;
using MediatR;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IValidator<UpdateTaskCommand> _validator; // Добавили валидатор
    private readonly ICacheService _cacheService; // Добавляем кэш

    public UpdateTaskCommandHandler(IAppDbContext context, IValidator<UpdateTaskCommand> validator, ICacheService cacheService)
    {
        _context = context;
        _validator = validator;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        //  Валидация входных данных
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        //  Загружаем задачу из БД
        var task = await _context.Tasks.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (task == null) return false;

        //  Обновляем данные
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = (TaskStatus)request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Обновляем кэш (перезаписываем задачу)
        string cacheKey = $"tasks";
        await _cacheService.SetAsync(cacheKey, task, TimeSpan.FromMinutes(10));

        return true;
    }
}