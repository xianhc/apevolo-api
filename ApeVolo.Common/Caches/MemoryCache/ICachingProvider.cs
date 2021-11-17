namespace ApeVolo.Common.Caches.MemoryCache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICaching
    {
        object Get(string cacheKey);

        void Set(string cacheKey, object cacheValue);
    }
}
