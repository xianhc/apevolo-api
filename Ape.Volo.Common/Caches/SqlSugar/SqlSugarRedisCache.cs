using System;
using System.Collections.Generic;
using System.Threading;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Redis;

namespace Ape.Volo.Common.Caches.SqlSugar;

/// <summary>
/// redis缓存
/// 实现SqlSugar.ICacheService
/// </summary>
public class SqlSugarRedisCache : ICacheService
{
    private static IDatabase _database;

    public SqlSugarRedisCache()
    {
        ThreadPool.SetMinThreads(200, 200);
        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = AppSettings.GetValue<bool>("Redis", "AbortOnConnectFail"),
            AllowAdmin = AppSettings.GetValue<bool>("Redis", "AllowAdmin"),
            ConnectRetry = AppSettings.GetValue<int>("Redis", "ConnectRetry"),
            ConnectTimeout = AppSettings.GetValue<int>("Redis", "ConnectTimeout"),
            KeepAlive = AppSettings.GetValue<int>("Redis", "KeepAlive"),
            SyncTimeout = AppSettings.GetValue<int>("Redis", "SyncTimeout"),
            EndPoints =
            {
                AppSettings.GetValue<string>("Redis", "Host") + ":" + AppSettings.GetValue<int>("Redis", "Port")
            },
            ServiceName = AppSettings.GetValue<string>("Redis", "Name") + "_SqlSugarCache",
        };
        if (!string.IsNullOrWhiteSpace(AppSettings.GetValue<string>("Redis", "Password")))
        {
            options.Password = AppSettings.GetValue<string>("Redis", "Password");
        }

        var connection = ConnectionMultiplexer.Connect(options);
        _database = connection.GetDatabase(AppSettings.GetValue<int>("Redis", "Index") + 1);
    }

    public void Add<V>(string key, V value)
    {
        var valStr = value.ToJson();
        _database.StringSet(key, valStr);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        var valStr = value.ToJson();
        var expireTime = new TimeSpan(0, 0, 0, cacheDurationInSeconds);
        _database.StringSet(key, valStr, expireTime);
    }

    public bool ContainsKey<V>(string key)
    {
        return _database.KeyExists(key);
    }

    public V Get<V>(string key)
    {
        var val = _database.StringGet(key);
        var redisValue = _database.StringGet(key);
        if (!redisValue.HasValue)
            return default;
        return JsonConvert.DeserializeObject<V>(val);
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        var pattern = "SqlSugarDataCache.*";
        var redisResult = _database.ScriptEvaluate(LuaScript.Prepare(
            " local res = redis.call('KEYS', @keypattern) " +
            " return res "), new { keypattern = pattern });
        string[] preSult = (string[])redisResult;
        return preSult;
    }

    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        if (ContainsKey<V>(cacheKey))
        {
            return Get<V>(cacheKey);
        }

        V val = create();
        Add(cacheKey, val, cacheDurationInSeconds);
        return val;
    }

    public void Remove<V>(string key)
    {
        _database.KeyDelete(key);
    }
}
