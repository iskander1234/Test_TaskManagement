using MediatR;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICacheService _cacheService;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // Проверяем, существует ли задача в БД
        var task = await _taskRepository.GetByIdAsync(request.Id);
        if (task is null) return false;

        // Удаляем задачу через Dapper
        await _taskRepository.DeleteAsync(request.Id);

        // Удаляем из Redis
        string cacheKey = $"task_{request.Id}";
        await _cacheService.RemoveAsync(cacheKey);
        Console.WriteLine($"Удалено из Redis: {cacheKey}");


        return true;
    }
}