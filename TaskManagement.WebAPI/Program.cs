using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using TaskManagement.Infrastructure;
using TaskManagement.WebAPI.Middlewares;
using Serilog;
using TaskManagement.Application.Background;
using TaskManagement.Application.Evets;
using TaskManagement.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Подключаем все уровни
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

// Добавляем контроллеры и SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключаем Hangfire с PostgreSQL
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Добавляем BackgroundJobs в DI-контейнер
builder.Services.AddScoped<BackgroundJobs>();

// Настраиваем Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

//  Добавляем RabbitMQ через MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TaskCreatedEventHandler>(); // Регистрируем Consumer

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Добавляем очередь
        cfg.ReceiveEndpoint("task-created-queue", e =>
        {
            e.ConfigureConsumer<TaskCreatedEventHandler>(context);
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Добавляем маршрутизацию перед использованием эндпоинтов!
app.UseRouting();

// Middleware для обработки ошибок
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

// Настраиваем маршруты правильно
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<TaskHub>("/taskHub"); // Добавляем SignalR хаб
});

// Добавляем Hangfire Dashboard
app.UseHangfireDashboard();

// Регистрируем фоновое задание после инициализации Hangfire
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Запускаем задание каждую ночь в 00:00 (UTC)
    recurringJobManager.AddOrUpdate(
        "archive-old-tasks",
        () => scope.ServiceProvider.GetRequiredService<BackgroundJobs>().ArchiveOldTasks(),
        Cron.Daily);
}

app.Run();