using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Ape.Volo.Common.Caches;

public interface ICache
{
    IDatabase GetDatabase();

    #region 获取缓存

    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T Get<T>(string key);

    /// <summary>
    /// 获取缓存数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T> GetAsync<T>(string key);

    #endregion

    #region 添加缓存

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeout">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    bool Set(string key, object value, TimeSpan? timeout, CacheExpireType? redisExpireType);

    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="timeout">过期时间</param>
    /// <param name="redisExpireType">过期类型</param>
    /// <returns>添加结果</returns>
    Task<bool> SetAsync(string key, object value, TimeSpan? timeout, CacheExpireType? redisExpireType);

    #endregion

    #region 移除缓存

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>移除结果</returns>
    bool Remove(string key);

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>移除结果</returns>
    Task<bool> RemoveAsync(string key);

    #endregion

    #region 模糊查询key

    /// <summary>
    /// 模糊查询key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string[]> ScriptEvaluateKeys(string key);

    #endregion
}
