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

    public UpdateTaskCommandHandler(IAppDbContext context, IValidator<UpdateTaskCommand> validator)
    {
        _context = context;
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
        
        var task = await _context.Tasks.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (task == null) return false;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = (TaskStatus)request.Status;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}