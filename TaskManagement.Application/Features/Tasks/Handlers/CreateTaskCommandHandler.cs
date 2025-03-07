using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;


namespace TaskManagement.Application.Features.Tasks.Handlers;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateTaskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Tasks.AnyAsync(t => t.Title == request.Title, cancellationToken))
        {
            throw new Exception("Task with this title already exists.");
        }

        var task = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = Entities.TaskStatus.ToDo
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}