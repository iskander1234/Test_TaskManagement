using MediatR;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cacheService; // Добавляем кэш

    public DeleteTaskCommandHandler(IAppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FindAsync(new object[] { request.Id }, cancellationToken);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Очистка кэша после удаления задачи
        await _cacheService.RemoveAsync("tasks");
        
        return true;
    }
}