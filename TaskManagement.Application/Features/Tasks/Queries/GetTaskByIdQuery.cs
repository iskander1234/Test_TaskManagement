using MediatR;
using TaskManagement.Application.Entities;

namespace TaskManagement.Application.Features.Tasks.Queries;

public class GetTaskByIdQuery : IRequest<TaskEntity?>
{
    public Guid Id { get; }

    public GetTaskByIdQuery(Guid id)
    {
        Id = id;
    }
}