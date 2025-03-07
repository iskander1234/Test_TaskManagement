using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.WebAPI.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; 

// Используем Dependency Injection из Application и Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Используем Serilog
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

app.Run();