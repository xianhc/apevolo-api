using ApeVolo.Common.Caches.Redis.Models;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ApeVolo.Common.Caches.Redis.Extensions
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public static class RedisHelper
    {
      /*  #region 初始化参数

        private static readonly int DefulatTime = 20; //默认有效期 20分钟
        private static RedisOptions _config;
        private static ConnectionMultiplexer _connection;
        private static IDatabase _db;
        private static ISubscriber _sub;

        #endregion

        public static IServiceCollection AddRedisCacheService(
            this IServiceCollection serviceCollection,
            Func<RedisOptions, RedisOptions> redisOptions = null
        )
        {
            var redisOptionsNew = new RedisOptions();
            redisOptionsNew = redisOptions?.Invoke(redisOptionsNew) ?? redisOptionsNew;
            _config = redisOptionsNew;
            _connection = ConnectionMultiplexer.Connect(GetSystemOptions());
            _db = _connection.GetDatabase(_config.RedisIndex);
            _sub = _connection.GetSubscriber();
            return serviceCollection;
        }

        #region 系统配置

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        private static ConfigurationOptions GetSystemOptions()
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


        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> GetCacheStrAsync<T>(string key) where T : class
        {
            return (T) await GetCacheStrAsync(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static async Task<object> GetCacheStrAsync(string key)
        {
            object value = null;
            var redisValue = await _db.StringGetAsync(key);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> GetCacheAsync<T>(string key) where T : class
        {
            return (T) await GetCacheAsync(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static async Task<object> GetCacheAsync(string key)
        {
            object value = null;
            var redisValue = await _db.StringGetAsync(key);
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
        public static async Task<bool> ExpireAsync(string key, TimeSpan expire)
        {
            return await _db.KeyExpireAsync(key, expire);
        }

        #endregion

        #region 添加缓存

        /// <summary>
        /// 添加缓存[异步]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>添加结果</returns>
        public static async Task<bool> SetCacheAsync(string key, object value)
        {
            return await InsertCacheAsync(key, value, new TimeSpan?(TimeSpan.FromMinutes(DefulatTime)),
                RedisExpireType.Absolute);
        }

        /// <summary>
        /// 添加缓存[异步]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <returns>添加结果</returns>
        public static async Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout)
        {
            return await InsertCacheAsync(key, value, timeout, RedisExpireType.Absolute);
        }

        /// <summary>
        /// 添加缓存[异步]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <param name="redisExpireType">过期类型</param>
        /// <returns>添加结果</returns>
        public static async Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout,
            RedisExpireType redisExpireType)
        {
            return await InsertCacheAsync(key, value, timeout, redisExpireType);
        }

        /// <summary>
        /// 添加缓存[异步]
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <param name="redisExpireType">过期类型</param>
        /// <returns>添加结果</returns>
        private static async Task<bool> InsertCacheAsync(string key, object value, TimeSpan? timeout,
            RedisExpireType? redisExpireType)
        {
            string jsonStr = string.Empty;
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
                return await _db.StringSetAsync(key, theValue);
            else
                return await _db.StringSetAsync(key, theValue, timeout);
        }

        #endregion

        #region 移除缓存

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>移除结果</returns>
        public static async Task<bool> RemoveAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }

        #endregion

        #region 数据转换

        /// <summary>
        /// JSON转换配置文件
        /// </summary>
        private static JsonSerializerSettings _jsoncfg = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        /// <summary>
        /// 封装模型转换为字符串进行存储
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string ToJson(object value)
        {
            return ToJson<object>(value);
        }

        /// <summary>
        /// 封装模型转换为字符串进行存储
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string ToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(new RedisData<T>
            {
                Value = value
            }, _jsoncfg);
        }

        /// <summary>
        /// 缓存字符串转为封装模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static RedisData<T> JsonTo<T>(string value)
        {
            return JsonConvert.DeserializeObject<RedisData<T>>(value, _jsoncfg);
        }

        private static T Recieve<T>(string cachevalue)
        {
            T result = default;
            bool flag = !string.IsNullOrWhiteSpace(cachevalue);
            if (flag)
            {
                var cacheObject = JsonConvert.DeserializeObject<RedisData<T>>(cachevalue, _jsoncfg);
                result = cacheObject.Value;
            }

            return result;
        }

        #endregion

        #region 队列

        public static async Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            return await _db.ListLeftPushAsync(redisKey, redisValue);
        }

        #endregion

        #region 方法委托

        /// <summary>
        /// 委托执行方法
        /// </summary>
        /// <param name="d"></param>
        public delegate void DoSub(object d);

        #endregion

        #region 其他

        /// <summary>
        /// 通配符匹配keys
        /// </summary>
        /// <returns></returns>
        public static async Task<string[]> FuzzyMatchingKeyList(string keyword)
        {
            var pattern = $"{keyword}*"; //匹配符
            var redisResult = await _db.ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new {@keypattern = pattern});
            string[] preSult = (string[]) redisResult; //将返回的结果集转为数组
            return preSult;
        }

        #endregion

        #region redis配置

        /// <summary>
        /// redis配置
        /// </summary>
        /// <returns></returns>
        public static RedisOptions GetRedisOptions()
        {
            RedisOptions redisOptions = new RedisOptions()
            {
                RedisHost = AppSettings.GetValue(new[] {"RedisConfig", "RedisHost"}),

                Port = AppSettings.GetValue(new[] {"RedisConfig", "Port"}),

                RedisName = AppSettings.GetValue(new[] {"RedisConfig", "RedisName"}),

                RedisPass = AppSettings.GetValue(new[] {"RedisConfig", "RedisPass"}),

                RedisIndex = AppSettings.GetValue(new[] {"RedisConfig", "RedisIndex"}).ToInt(),

                ConnectTimeout = AppSettings.GetValue(new[] {"RedisConfig", "SyncTimeout"}).ToInt(),

                SyncTimeout = AppSettings.GetValue(new[] {"RedisConfig", "ConnectTimeout"}).ToInt(),

                KeepAlive = AppSettings.GetValue(new[] {"RedisConfig", "KeepAlive"}).ToInt(),

                ConnectRetry = AppSettings.GetValue(new[] {"RedisConfig", "ConnectRetry"}).ToInt(),

                AbortOnConnectFail = AppSettings.GetValue(new[] {"RedisConfig", "AbortOnConnectFail"}).ToBool(),

                AllowAdmin = AppSettings.GetValue(new[] {"RedisConfig", "AllowAdmin"}).ToBool()
            };
            return redisOptions;
        }

        #endregion*/
    }
}