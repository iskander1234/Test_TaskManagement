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
    private readonly ICacheService _cacheService; // Подключаем сервис кэша

    public GetAllTasksQueryHandler(IAppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<TaskEntity>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = "tasks";

        //Проверяем, есть ли данные в кэше
        var cachedTasks = await _cacheService.GetAsync<List<TaskEntity>>(cacheKey);
        if (cachedTasks is not null)
        {
            Console.WriteLine("Данные получены из кэша!");
            return cachedTasks;
        }

        // Данных нет, загружаем из БД
        var query = _context.Tasks.AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == (TaskStatus)request.Status.Value);
        }

        var tasks = await query.ToListAsync(cancellationToken);

        //Сохраняем данные в Redis на 10 минут
        await _cacheService.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(10));

        Console.WriteLine("Данные сохранены в Redis!");

        return tasks;
    }
}