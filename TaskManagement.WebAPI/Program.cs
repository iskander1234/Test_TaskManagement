using Hangfire;
using Hangfire.PostgreSql;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.WebAPI.Middlewares;
using Serilog;
using TaskManagement.Application.Background;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Подключаем все уровни
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключаем Hangfire с PostgreSQL
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// 📌 Добавляем BackgroundJobs в DI-контейнер
builder.Services.AddScoped<BackgroundJobs>();

// Настраиваем Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware для обработки ошибок
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ✅ Добавляем Hangfire Dashboard
app.UseHangfireDashboard();

// ✅ Регистрируем фоновое задание после инициализации Hangfire
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // 📌 Запускаем задание каждую ночь в 00:00 (UTC)
    recurringJobManager.AddOrUpdate(
        "archive-old-tasks",
        () => scope.ServiceProvider.GetRequiredService<BackgroundJobs>().ArchiveOldTasks(),
        Cron.Daily);
}

app.Run();