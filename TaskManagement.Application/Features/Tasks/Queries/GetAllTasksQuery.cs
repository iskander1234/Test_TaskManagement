using MediatR;
using TaskManagement.Application.Entities;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Queries;

public class GetAllTasksQuery : IRequest<List<TaskEntity>>
{
    public TaskStatus? Status { get; set; }

    public GetAllTasksQuery(TaskStatus? status)
    {
        Status = status;
    }
}