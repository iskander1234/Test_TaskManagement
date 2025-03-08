using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<TaskEntity> Tasks { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(255);
            entity.Property(t => t.Description).IsRequired(false);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(t => t.UpdatedAt).HasDefaultValueSql("NOW()");
        });
    }
}