# Task Management API

## 📌 Описание проекта
Task Management API — это RESTful API для управления задачами, разработанное с использованием **ASP.NET Core**, **PostgreSQL**, **Dapper**, **Entity Framework Core**, **Hangfire**, **RabbitMQ** и **SignalR**.

### 🔧 Технологии и инструменты
- **C# .NET 8**
- **Entity Framework Core + Dapper** (работа с БД)
- **PostgreSQL** (хранение данных)
- **Redis** (кэширование)
- **MassTransit + RabbitMQ** (обмен событиями)
- **Hangfire** (фоновые задачи)
- **FluentValidation** (валидация данных)
- **AutoMapper** (маппинг моделей)
- **Swagger** (документация API)
- **Serilog** (логирование)
- **SignalR** (уведомления в реальном времени)

## 🚀 Установка и запуск

### 1️⃣ Клонирование репозитория
```sh
git clone https://github.com/your-repo/task-management-api.git
cd task-management-api
```

### 2️⃣ Настройка базы данных PostgreSQL
Создайте базу данных **TaskManagementDb2**.

### 3️⃣ Конфигурация `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=TaskManagementDb2;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Enabled": true,
    "ConnectionString": "localhost:6379",
    "CacheExpirationMinutes": 10
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### 4️⃣ Запуск миграций
```sh
dotnet ef database update -p TaskManagement.Infrastructure -s TaskManagement.WebAPI
```

### 5️⃣ Запуск API
```sh
dotnet run --project TaskManagement.WebAPI
```

## 📄 Документация API
После запуска откройте Swagger:
```
http://localhost:5057/swagger/index.html
```

## 🛠 Тестирование API через Postman

📥 **Импортируйте Postman Collection**:
1. Откройте Postman.
2. Перейдите в **File → Import**.
3. Выберите `TaskManagement.postman_collection.json`.
4. Запустите тестовые запросы!

Файл Postman Collection: **[TaskManagement.postman_collection.json](TaskManagement.postman_collection.json)**.

## 🔗 Дополнительные инструменты
- **Hangfire Dashboard**: `http://localhost:5057/hangfire`
- **RabbitMQ Management UI**: `http://localhost:15672/` (логин: `guest`, пароль: `guest`)
- **SignalR Hub**: `http://localhost:5057/taskHub`

## Контакты
Автор: **Искандер**  
Email: **idosgali@gmail.com**

