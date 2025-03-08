using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Caching;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Подключаем PostgreSQL для EF Core
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Подключаем Dapper
        services.AddScoped<IDbConnection>(sp =>
            new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ITaskRepository, DapperTaskRepository>();

        // Подключаем Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetValue<string>("Redis:ConnectionString");
        });

        // Добавляем сервис кэша
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        services.AddScoped<ISignalRService, SignalRService>();
        services.AddSignalR(); // Подключаем SignalR

    }
}