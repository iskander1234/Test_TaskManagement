using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands;

public class DeleteTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteTaskCommand(Guid id)
    {
        Id = id;
    }
}