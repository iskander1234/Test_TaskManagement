namespace TaskManagement.Application.Interfaces;

public interface ISignalRService
{
    Task SendTaskUpdate(string taskId, string title, int status);
}