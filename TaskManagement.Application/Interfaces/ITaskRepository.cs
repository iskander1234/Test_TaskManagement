using TaskManagement.Application.Entities;

namespace TaskManagement.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllAsync(int? status = null);
    Task<TaskEntity?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(TaskEntity task);
    Task<bool> UpdateAsync(TaskEntity task);
    Task<bool> DeleteAsync(Guid id);
}