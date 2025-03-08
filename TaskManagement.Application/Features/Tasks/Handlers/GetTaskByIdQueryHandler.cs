using MediatR;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Handlers;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskEntity?>
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cacheService; // Подключаем сервис кэша

    public GetTaskByIdQueryHandler(IAppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<TaskEntity?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"tasks"; // Уникальный ключ для кэша

        // Проверяем кэш перед запросом в БД
        var cachedTask = await _cacheService.GetAsync<TaskEntity>(cacheKey);
        if (cachedTask != null)
        {
            return cachedTask; // Возвращаем кэшированный объект
        }

        // Загружаем задачу из БД
        var task = await _context.Tasks.FindAsync(new object[] { request.Id }, cancellationToken);
        if (task == null) return null;

        // Сохраняем в кэш (один объект, не список!)
        await _cacheService.SetAsync(cacheKey, task, TimeSpan.FromMinutes(10));

        return task;
    }
}