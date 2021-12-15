using System;
using System.Threading;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Newtonsoft.Json;
using StackExchange.Redis;
using RedisKey = StackExchange.Redis.RedisKey;

namespace ApeVolo.Common.Caches.Redis.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private static int _defaultTimeout = 60 * 20; //默认超时时间（单位秒）
        private static RedisOptions _config;
        private static IDatabase _database;
        private static ISubscriber _sub;

        public RedisCacheService(RedisOptions redisOptions)
        {
            if (redisOptions.IsNull())
            {
                throw new System.Exception("redis初始化失败！！！");
            }

            //设置线程池最小连接数
            ThreadPool.SetMinThreads(200, 200);
            _config = redisOptions;
            var connection = ConnectionMultiplexer.Connect(GetRedisOptions());
            _database = connection.GetDatabase(_config.RedisIndex);
            _sub = connection.GetSubscriber();
        }

        #region 获取redis配置

        /// <summary>
        /// 获取redis配置
        /// </summary>
        /// <returns></returns>
        private static ConfigurationOptions GetRedisOptions()
        {
            var options = new ConfigurationOptions
            {
                AbortOnConnectFail = _config.AbortOnConnectFail,
                AllowAdmin = _config.AllowAdmin,
                ConnectRetry = _config.ConnectRetry, //10,
                ConnectTimeout = _config.ConnectTimeout,
                KeepAlive = _config.KeepAlive,
                SyncTimeout = _config.SyncTimeout,
                EndPoints = {_config.RedisHost + ":" + _config.Port},
                ServiceName = _config.RedisName,
            };
            if (!string.IsNullOrWhiteSpace(_config.RedisPass))
            {
                options.Password = _config.RedisPass;
            }

            return options;
        }

        #endregion
        
        public IDatabase GetDatabase()
        {
            return _database;
        }

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetCacheStrAsync<T>(string key)
        {
            return (T) await GetCacheStrAsync(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetCacheAsync<T>(string key)
        {
            return (T) await GetCacheAsync(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<object> GetCacheStrAsync(string key)
        {
            object value = null;
            var redisValue = await _database.StringGetAsync(key);
            if (!redisValue.HasValue)
                return null;
            ValueInfoEntry valueEntry = redisValue.ToString().ToObject<ValueInfoEntry>();
            value = valueEntry.Value;
            if (valueEntry.ExpireTime != null && valueEntry.ExpireType == RedisExpireType.Relative)
                await ExpireAsync(key, valueEntry.ExpireTime.Value);

            return value;
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<object> GetCacheAsync(string key)
        {
            object value = null;
            var redisValue = await _database.StringGetAsync(key);
            if (!redisValue.HasValue)
                return null;
            ValueInfoEntry valueEntry = redisValue.ToString().ToObject<ValueInfoEntry>();
            if (valueEntry.TypeName == typeof(string).AssemblyQualifiedName)
                value = valueEntry.Value;
            else
                value = valueEntry.Value.ToObject(Type.GetType(valueEntry.TypeName));

            if (valueEntry.ExpireTime != null && valueEntry.ExpireType == RedisExpireType.Relative)
                await ExpireAsync(key, valueEntry.ExpireTime.Value);

            return value;
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public async Task<bool> ExpireAsync(string key, TimeSpan expire)
        {
            return await _database.KeyExpireAsync(key, expire);
        }

        #endregion

        #region 添加缓存

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>添加结果</returns>
        public async Task<bool> SetCacheAsync(string key, object value)
        {
            return await InsertCacheAsync(key, value, TimeSpan.FromSeconds(_defaultTimeout),
                RedisExpireType.Absolute);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <returns>添加结果</returns>
        public async Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout)
        {
            return await InsertCacheAsync(key, value, timeout, RedisExpireType.Absolute);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <param name="redisExpireType">过期类型</param>
        /// <returns>添加结果</returns>
        public async Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout,
            RedisExpireType redisExpireType)
        {
            return await InsertCacheAsync(key, value, timeout, redisExpireType);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <param name="redisExpireType">过期类型</param>
        /// <returns>添加结果</returns>
        public async Task<bool> InsertCacheAsync(string key, object value, TimeSpan? timeout,
            RedisExpireType? redisExpireType)
        {
            string jsonStr;
            if (value is string)
                jsonStr = value as string;
            else
                jsonStr = value.ToRedisJson();

            ValueInfoEntry entry = new ValueInfoEntry
            {
                Value = jsonStr,
                TypeName = value.GetType().AssemblyQualifiedName,
                ExpireTime = timeout,
                ExpireType = redisExpireType
            };

            string theValue = entry.ToRedisJson();
            if (timeout == null)
                return await _database.StringSetAsync(key, theValue);
            return await _database.StringSetAsync(key, theValue, timeout);
        }

        #endregion

        #region 移除缓存

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
                " return res "), new {keypattern = pattern});
            string[] preSult = (string[]) redisResult; //将返回的结果集转为数组
            return preSult;
        }
        
        public long ListLeftPush(string key, string value)
        {
            var res = _database.ListLeftPush(key, value);
            return res;
        }

        public async Task<long> ListLeftPushAsync(string key, string value)
        {
            var res = await _database.ListLeftPushAsync(key, value);
            return res;
        }

        public long ListRightPush(string key, string value)
        {
            var res = _database.ListRightPush(key, value);
            return res;
        }

        public async Task<long> ListRightPushAsync(string key, string value)
        {
            var res = await _database.ListRightPushAsync(key, value);
            return res;
        }

        public T ListLeftPop<T>(string key) where T : class
        {
            var cacheValue = _database.ListLeftPop(key);
            if (string.IsNullOrEmpty(cacheValue)) return null;
            var res = JsonConvert.DeserializeObject<T>(cacheValue);
            return res;
        }

        public async Task<T> ListLeftPopAsync<T>(string key) where T : class
        {
            var cacheValue = await _database.ListLeftPopAsync(key);
            if (string.IsNullOrEmpty(cacheValue)) return null;
            var res = JsonConvert.DeserializeObject<T>(cacheValue);
            return res;
        }


        public T ListRightPop<T>(string key) where T : class
        {
            var cacheValue = _database.ListRightPop(key);
            if (string.IsNullOrEmpty(cacheValue)) return null;
            var res = JsonConvert.DeserializeObject<T>(cacheValue);
            return res;
        }

        public async Task<T> ListRightPopAsync<T>(string key) where T : class
        {
            var cacheValue = await _database.ListRightPopAsync(key);
            if (string.IsNullOrEmpty(cacheValue)) return null;
            var res = JsonConvert.DeserializeObject<T>(cacheValue);
            return res;
        }


        public string ListLeftPop(string key)
        {
            var cacheValue = _database.ListLeftPop(key);
            return cacheValue;
        }

        public async Task<string> ListLeftPopAsync(string key)
        {
            var cacheValue = await _database.ListLeftPopAsync(key);
            return cacheValue;
        }


        public string ListRightPop(string key)
        {
            var cacheValue = _database.ListRightPop(key);
            return cacheValue;
        }

        public async Task<string> ListRightPopAsync(string key)
        {
            var cacheValue = await _database.ListRightPopAsync(key);
            return cacheValue;
        }


        public long ListLength(string key)
        {
            return _database.ListLength(key);
        }

        public async Task<long> ListLengthAsync(string key)
        {
            return await _database.ListLengthAsync(key);
        }


        public long Publish(string key, string msg)
        {
            var count = _database.Publish(key, msg);
            return count;
        }

        public async Task<long> PublishAsync(string key, string msg)
        {
            var count = await _database.PublishAsync(key, msg);
            return count;
        }


        public void Subscribe(string key, Action<RedisChannel, RedisValue> action)
        {
            _sub.Subscribe(key, action);
        }

        public async Task SubscribeAsync(string key, Action<RedisChannel, RedisValue> action)
        {
            await _sub.SubscribeAsync(key, action);
        }


        public async Task<bool> SortedSetAddAsync(string key, string msg, double score)
        {
            var bl = await _database.SortedSetAddAsync(key, msg, score);
            return bl;
        }


        public async Task<string[]> SortedSetRangeByScoreAsync(string key, double start = double.NegativeInfinity,
            double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending)
        {
            var arry = await _database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order);
            return arry.ToStringArray();
        }


        public async Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop)
        {
            var bl = await _database.SortedSetRemoveRangeByScoreAsync(key, start, stop);
            return bl;
        }

        public async Task<bool> SortedSetAddAsync(string key, string msg, DateTime time)
        {
            var score = (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            var bl = await _database.SortedSetAddAsync(key, msg, score);
            return bl;
        }


        public async Task<string[]> SortedSetRangeByScoreAsync(string key, DateTime? startTime, DateTime? stopTime,
            Exclude exclude = Exclude.None, Order order = Order.Ascending)
        {
            var start = double.NegativeInfinity;
            var stop = double.PositiveInfinity;
            if (startTime.HasValue)
            {
                start = (startTime.Value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }

            if (stopTime.HasValue)
            {
                stop = (stopTime.Value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }

            var arry = await _database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order);
            return arry.ToStringArray();
        }


        public async Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, DateTime? startTime, DateTime? stopTime)
        {
            var start = double.NegativeInfinity;
            var stop = double.PositiveInfinity;
            if (startTime.HasValue)
            {
                start = (startTime.Value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }

            if (stopTime.HasValue)
            {
                stop = (stopTime.Value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            }

            var bl = await _database.SortedSetRemoveRangeByScoreAsync(key, start, stop);
            return bl;
        }
    }
}