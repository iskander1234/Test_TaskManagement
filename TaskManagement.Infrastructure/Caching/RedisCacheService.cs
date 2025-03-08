using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Entities;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly bool _enabled;
    private readonly int _expirationMinutes;

    public RedisCacheService(IDistributedCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _enabled = configuration.GetValue<bool>("Redis:Enabled");
        _expirationMinutes = configuration.GetValue<int>("Redis:CacheExpirationMinutes");
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        if (!_enabled) return;

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var jsonData = JsonSerializer.Serialize(value); // JSON-строка
        Console.WriteLine($"Сохраняем в Redis: {jsonData}"); // Лог
        await _cache.SetStringAsync(key, jsonData, options); // Используем SetStringAsync
    }


    public async Task<T?> GetAsync<T>(string key)
    {
        if (!_enabled) return default;

        var jsonData = await _cache.GetStringAsync(key);
        Console.WriteLine($"Получено из Redis [{key}]: {jsonData}");

        return jsonData is not null ? JsonSerializer.Deserialize<T>(jsonData) : default;
    }



    public async Task RemoveAsync(string key)
    {
        if (!_enabled) return;
        await _cache.RemoveAsync(key);
    }
}
