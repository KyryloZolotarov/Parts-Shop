﻿using Basket.Host.Configurations;
using Basket.Host.Services.Interfaces;
using StackExchange.Redis;

namespace Basket.Host.Services;

public class CacheService : ICacheService
{
    private readonly RedisConfig _config;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<CacheService> _logger;
    private readonly IRedisCacheConnectionService _redisCacheConnectionService;

    public CacheService(
        ILogger<CacheService> logger,
        IRedisCacheConnectionService redisCacheConnectionService,
        IOptions<RedisConfig> config,
        IJsonSerializer jsonSerializer)
    {
        _logger = logger;
        _redisCacheConnectionService = redisCacheConnectionService;
        _jsonSerializer = jsonSerializer;
        _config = config.Value;
    }

    public Task AddOrUpdateAsync<T>(string key, T value)
    {
        return AddOrUpdateInternalAsync(key, value);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var redis = GetRedisDatabase();

        var cacheKey = GetItemCacheKey(key);

        var serialized = await redis.StringGetAsync(cacheKey);
        _logger.LogInformation($"Value in basket {serialized.ToString()}");

        return serialized.HasValue
            ? _jsonSerializer.Deserialize<T>(serialized.ToString())
            : default!;
    }

    public Task Delete(string basketId)
    {
        var redis = GetRedisDatabase();
        var cacheKey = GetItemCacheKey(basketId);
        return redis.KeyDeleteAsync(cacheKey);
    }

    private string GetItemCacheKey(string userId)
    {
        return $"{userId}";
    }

    private async Task AddOrUpdateInternalAsync<T>(string key, T value,
        IDatabase redis = null!, TimeSpan? expiry = null)
    {
        redis = redis ?? GetRedisDatabase();
        expiry = expiry ?? _config.CacheTimeout;

        var cacheKey = GetItemCacheKey(key);
        var serialized = _jsonSerializer.Serialize(value);

        if (await redis.StringSetAsync(cacheKey, serialized, expiry))
            _logger.LogInformation($"Cached value for key {key} cached");
        else
            _logger.LogInformation($"Cached value for key {key} updated");
    }

    private IDatabase GetRedisDatabase()
    {
        return _redisCacheConnectionService.Connection.GetDatabase();
    }
}