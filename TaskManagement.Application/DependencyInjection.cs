using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        // Регистрируем MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Регистрируем FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Регистрируем AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}