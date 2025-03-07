using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using MediatR;
using TaskManagement.Application.Features.Tasks.Handlers;
using TaskManagement.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // ✅ Добавляем конфигурацию

// Подключаем PostgreSQL
builder.Services.AddDbContext<IAppDbContext, AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Подключаем MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommandHandler).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();