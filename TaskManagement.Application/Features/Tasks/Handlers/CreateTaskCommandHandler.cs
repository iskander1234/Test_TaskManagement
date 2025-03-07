using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;


namespace TaskManagement.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly IValidator<CreateTaskCommand> _validator; // Добавили валидатор
    private readonly IMapper _mapper; // Добавили AutoMapper
    public CreateTaskCommandHandler(IAppDbContext context, IValidator<CreateTaskCommand> validator , IMapper mapper)
    {
        _context = context;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Выполняем валидацию
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Проверяем дубликат Title
        if (await _context.Tasks.AnyAsync(t => t.Title == request.Title, cancellationToken))
        {
            throw new Exception("Task with this title already exists.");
        }
        
        // Используем AutoMapper для преобразования команды в сущность
        var task = _mapper.Map<TaskEntity>(request);
        task.Id = Guid.NewGuid();
        task.Status = TaskStatus.ToDo;
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}