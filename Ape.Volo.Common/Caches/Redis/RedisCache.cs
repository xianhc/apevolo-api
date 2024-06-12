using System;
using System.Threading;
using System.Threading.Tasks;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extensions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Ape.Volo.Common.Caches.Redis;

public class RedisCache : ICache
{
    private const int DefaultTimeout = 60 * 20;
    private readonly Configs _configs;

    private static IDatabase _database;
    //private static ISubscriber _sub;

    public RedisCache(IOptionsMonitor<Configs> configs)
    {
        _configs = configs?.CurrentValue ?? new Configs();
        if (!_configs.CacheOption.RedisCacheSwitch.Enabled)
        {
            throw new System.Exception("RedisCacheSwitch未开启,请检查！");
        }

        var redisConfigs = _configs.Redis;
        ThreadPool.SetMinThreads(200, 200);
        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = redisConfigs.AbortOnConnectFail,
            AllowAdmin = redisConfigs.AllowAdmin,
            ConnectRetry = redisConfigs.ConnectRetry, //10,
            ConnectTimeout = redisConfigs.ConnectTimeout,
            KeepAlive = redisConfigs.KeepAlive,
            SyncTimeout = redisConfigs.SyncTimeout,
            EndPoints = { redisConfigs.Host + ":" + redisConfigs.Port },
            ServiceName = redisConfigs.Name,
        };
        if (!string.IsNullOrWhiteSpace(redisConfigs.Password))
        {
            options.Password = redisConfigs.Password;
        }

        var connection = ConnectionMultiplexer.Connect(options);
        _database = connection.GetDatabase(redisConfigs.Index);
    }


    public IDatabase GetDatabase()
    {
        return _database;
    }

    #region 获取缓存

    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        object value = null;
        var redisValue = _database.StringGet(key);
        if (!redisValue.HasValue)
            return default;
        var valueEntry = redisValue.ToString().ToObject<ValueInfoEntry>();
        value = valueEntry.TypeName == typeof(string).AssemblyQualifiedName
            ? valueEntry.Value
            : valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

        if (valueEntry.ExpireType == CacheExpireType.Relative)
            _database.KeyExpire(key, valueEntry.ExpireTime);
        return (T)value;
    }


    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        object value = null;
        var redisValue = await _database.StringGetAsync(key);
        if (!redisValue.HasValue)
            return default;
        var valueEntry = redisValue.ToString().ToObject<ValueInfoEntry>();
        value = valueEntry.TypeName == typeof(string).AssemblyQualifiedName
            ? valueEntry.Value
            : valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

        if (valueEntry.ExpireType == CacheExpireType.Relative)
            await _database.KeyExpireAsync(key, valueEntry.ExpireTime);

        return (T)value;
    }

    #endregion

    #region 添加缓存

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeSpan">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    public bool Set(string key, object value, TimeSpan? timeSpan,
        CacheExpireType? redisExpireType)
    {
        string jsonStr;
        if (value is string s)
            jsonStr = s;
        else
            jsonStr = value.ToJson();
        var expireTime = timeSpan ?? new TimeSpan(0, 0, 0, DefaultTimeout);
        var entry = new ValueInfoEntry
        {
            Value = jsonStr,
            TypeName = value.GetType().AssemblyQualifiedName,
            ExpireTime = expireTime,
            ExpireType = redisExpireType ?? CacheExpireType.Absolute
        };
        var theValue = entry.ToJson();
        return _database.StringSet(key, theValue, expireTime);
    }


    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeSpan">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    public async Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan,
        CacheExpireType? redisExpireType)
    {
        string jsonStr;
        if (value is string s)
            jsonStr = s;
        else
            jsonStr = value.ToJson();
        var expireTime = timeSpan ?? new TimeSpan(0, 0, 0, DefaultTimeout);
        var entry = new ValueInfoEntry
        {
            Value = jsonStr,
            TypeName = value.GetType().AssemblyQualifiedName,
            ExpireTime = expireTime,
            ExpireType = redisExpireType ?? CacheExpireType.Absolute
        };
        var theValue = entry.ToJson();
        return await _database.StringSetAsync(key, theValue, expireTime);
    }

    #endregion

    #region 移除缓存

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>移除结果</returns>
    public bool Remove(string key)
    {
        return _database.KeyDelete(key);
    }

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>移除结果</returns>
    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    #endregion

    /// <summary>
    /// 模糊查询key的集合
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<string[]> ScriptEvaluateKeys(string key)
    {
        var pattern = $"{key}*"; //匹配符
        var redisResult = await _database.ScriptEvaluateAsync(LuaScript.Prepare(
            //Redis的keys模糊查询：
            " local res = redis.call('KEYS', @keypattern) " +
            " return res "), new { keypattern = pattern });
        string[] preSult = (string[])redisResult; //将返回的结果集转为数组
        return preSult;
    }
}
