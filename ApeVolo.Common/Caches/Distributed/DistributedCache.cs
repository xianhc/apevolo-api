using System;
using System.Text;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace ApeVolo.Common.Caches.Distributed;

public class DistributedCache : ICache
{
    private readonly IDistributedCache _cache;
    private const int DefaultTimeout = 60 * 20;

    public DistributedCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public IDatabase GetDatabase()
    {
        throw new NotImplementedException();
    }

    public T Get<T>(string key)
    {
        object result = null;
        var cacheValue = _cache.Get(key);
        if (cacheValue.IsNullOrEmpty())
            return default;
        var valueEntry = Encoding.UTF8.GetString(cacheValue, 0, cacheValue.Length).ToObject<ValueInfoEntry>();
        result = valueEntry.TypeName == typeof(string).AssemblyQualifiedName
            ? valueEntry.Value
            : valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

        if (valueEntry.ExpireType == CacheExpireType.Relative)
        {
            _cache.Remove(key);
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = valueEntry.ExpireTime
            };
            _cache.Set(key, cacheValue, options);
            //_cache.Refresh(key);
        }

        return (T)result;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        object result = null;
        var cacheValue = await _cache.GetAsync(key);
        if (cacheValue.IsNullOrEmpty())
            return default;
        var valueEntry = Encoding.UTF8.GetString(cacheValue, 0, cacheValue.Length).ToObject<ValueInfoEntry>();
        result = valueEntry.TypeName == typeof(string).AssemblyQualifiedName
            ? valueEntry.Value
            : valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

        if (valueEntry.ExpireType == CacheExpireType.Relative)
        {
            await _cache.RemoveAsync(key);
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = valueEntry.ExpireTime
            };
            await _cache.SetAsync(key, cacheValue, options);
            // await _cache.RefreshAsync(key);
        }

        return (T)result;
    }

    public bool Set(string key, object value, TimeSpan? timeSpan, CacheExpireType? redisExpireType)
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
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expireTime
        };
        _cache.Set(key, Encoding.UTF8.GetBytes(theValue), options);
        return true;
    }

    public async Task<bool> SetAsync(string key, object value, TimeSpan? timeSpan, CacheExpireType? redisExpireType)
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
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expireTime
        };
        await _cache.SetAsync(key, Encoding.UTF8.GetBytes(theValue), options);
        return true;
    }

    public bool Remove(string key)
    {
        _cache.Remove(key);
        return true;
    }

    public async Task<bool> RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        return true;
    }

    public Task<string[]> ScriptEvaluateKeys(string key)
    {
        throw new System.Exception("IDistributedCache不支持通配符查询,请切换redis缓存使用此功能");
    }
}
