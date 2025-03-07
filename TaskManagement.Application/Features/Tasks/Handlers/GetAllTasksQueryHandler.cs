using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Application.Entities.TaskStatus;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskEntity>>
{
    private readonly IAppDbContext _context;

    public GetAllTasksQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskEntity>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tasks.AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == (TaskStatus)request.Status.GetValueOrDefault());
        }

        return await query.ToListAsync(cancellationToken);
    }
}