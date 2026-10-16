using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UfgPay.Gateway.Application.Interfaces;

namespace UfgPay.Gateway.Infrastructure.Cache;

/// <summary>
/// Implementação de ICacheService integrada com Redis (StackExchange.Redis).
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[RedisCacheService.GetAsync] Buscando chave: {Key}", key);
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("[RedisCacheService.GetAsync] Cache MISS para chave: {Key}", key);
                return default;
            }

            _logger.LogDebug("[RedisCacheService.GetAsync] Cache HIT para chave: {Key}", key);
            return JsonSerializer.Deserialize<T>(value.ToString()!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RedisCacheService.GetAsync] Falha ao consultar Redis para chave: {Key}", key);
            return default; // Fallback tolerante a falhas
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[RedisCacheService.SetAsync] Gravando chave: {Key} com TTL: {TTL}", key, ttl);
        try
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RedisCacheService.SetAsync] Falha ao gravar no Redis para chave: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[RedisCacheService.RemoveAsync] Purgando chave: {Key}", key);
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RedisCacheService.RemoveAsync] Falha ao remover chave do Redis: {Key}", key);
        }
    }
}
