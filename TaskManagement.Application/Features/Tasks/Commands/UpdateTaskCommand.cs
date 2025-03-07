using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands;

public class UpdateTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
}