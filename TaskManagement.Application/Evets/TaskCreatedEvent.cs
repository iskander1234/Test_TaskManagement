namespace TaskManagement.Application.Evets;

public record TaskCreatedEvent(Guid TaskId, string Title)
{
}