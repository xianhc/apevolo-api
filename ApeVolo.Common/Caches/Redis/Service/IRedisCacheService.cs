using System;
using System.Threading.Tasks;
using ApeVolo.Common.Global;
using StackExchange.Redis;
using RedisKey = StackExchange.Redis.RedisKey;

namespace ApeVolo.Common.Caches.Redis.Service;

public interface IRedisCacheService
{
    IDatabase GetDatabase();

    #region 获取缓存

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> GetCacheStrAsync<T>(string key);

    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> GetCacheAsync<T>(string key);

    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    Task<object> GetCacheStrAsync(string key);


    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    Task<object> GetCacheAsync(string key);

    /// <summary>
    /// 设置过期时间
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="expire">过期时间</param>
    /// <returns></returns>
    Task<bool> ExpireAsync(string key, TimeSpan expire);

    #endregion

    #region 添加缓存

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>添加结果</returns>
    Task<bool> SetCacheAsync(string key, object value);

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeout">过期时间</param>
    /// <returns>添加结果</returns>
    Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout);

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeout">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    Task<bool> SetCacheAsync(string key, object value, TimeSpan timeout,
        RedisExpireType redisExpireType);

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeout">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    Task<bool> InsertCacheAsync(string key, object value, TimeSpan? timeout,
        RedisExpireType? redisExpireType);

    #endregion

    #region 移除缓存

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>移除结果</returns>
    Task<bool> RemoveAsync(string key);

    #endregion

    /// <summary>
    /// 模糊查询key的集合
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string[]> ScriptEvaluateKeys(string key);

    /// <summary>
    /// 入列
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<long> ListLeftPushAsync(string key, string value);

    long ListLeftPush(string key, string value);

    Task<long> ListRightPushAsync(string key, string value);

    long ListRightPush(string key, string value);

    /// <summary>
    /// 出列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> ListLeftPopAsync<T>(string key) where T : class;

    T ListLeftPop<T>(string key) where T : class;


    Task<T> ListRightPopAsync<T>(string key) where T : class;

    T ListRightPop<T>(string key) where T : class;

    Task<string> ListLeftPopAsync(string key);

    string ListLeftPop(string key);

    Task<string> ListRightPopAsync(string key);

    string ListRightPop(string key);

    /// <summary>
    /// 获取队列长度
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<long> ListLengthAsync(string key);

    long ListLength(string key);

    /// <summary>
    /// 通道广播
    /// </summary>
    /// <param name="key"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    long Publish(string key, string msg);

    /// <summary>
    /// 通道广播
    /// </summary>
    /// <param name="key"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    Task<long> PublishAsync(string key, string msg);

    /// <summary>
    /// 订阅通道
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    void Subscribe(string key, Action<RedisChannel, RedisValue> action);

    /// <summary>
    /// 订阅通道
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    Task SubscribeAsync(string key, Action<RedisChannel, RedisValue> action);

    /// <summary>
    /// 插入zset
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="msg">消息</param>
    /// <param name="score">序号</param>
    /// <returns></returns>
    Task<bool> SortedSetAddAsync(string key, string msg, double score);


    /// <summary>
    /// 插入zset
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="msg">消息</param>
    /// <param name="time">延迟执行时间</param>
    /// <returns></returns>
    Task<bool> SortedSetAddAsync(string key, string msg, DateTime time);

    /// <summary>
    /// 查询zset
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="start">序号开始</param>
    /// <param name="stop">序号结束</param>
    /// <param name="exclude"></param>
    /// <param name="order">排序</param>
    /// <returns></returns>
    Task<string[]> SortedSetRangeByScoreAsync(string key, double start = double.NegativeInfinity,
        double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending);


    /// <summary>
    /// 查询zset
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="startTime">延迟执行时间开始</param>
    /// <param name="stopTime">延迟执行时间结束</param>
    /// <param name="exclude"></param>
    /// <param name="order">排序</param>
    /// <returns></returns>
    Task<string[]> SortedSetRangeByScoreAsync(string key, DateTime? startTime, DateTime? stopTime,
        Exclude exclude = Exclude.None, Order order = Order.Ascending);


    /// <summary>
    /// 删除zset元素
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="start">序号开始</param>
    /// <param name="stop">序号结束</param>
    /// <returns></returns>
    Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop);

    /// <summary>
    /// 删除zset元素
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="startTime">延迟执行时间开始</param>
    /// <param name="stopTime">延迟执行时间结束</param>
    /// <returns></returns>
    Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, DateTime? startTime, DateTime? stopTime);
}