using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<TaskEntity> Tasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}