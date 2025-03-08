using Hangfire;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Background;

public class BackgroundJobs
{
    private readonly ITaskRepository _taskRepository;

    public BackgroundJobs(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [AutomaticRetry(Attempts = 3)] // Авто-повтор в случае ошибки
    public async Task ArchiveOldTasks()
    {
        var allTasks = await _taskRepository.GetAllAsync();
        var oldTasks = allTasks.Where(task => task.UpdatedAt < DateTime.UtcNow.AddDays(-30)).ToList();

        foreach (var task in oldTasks)
        {
            await _taskRepository.DeleteAsync(task.Id);
        }

        Console.WriteLine($"Архивировано {oldTasks.Count} задач");
    }
}