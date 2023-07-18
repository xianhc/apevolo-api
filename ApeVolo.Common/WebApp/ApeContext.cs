using System;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.ConfigOptions;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ApeVolo.Common.WebApp;

public class ApeContext
{
    private HttpContext _httpContext;

    public HttpContext HttpContext
    {
        get => _httpContext;
    }

    private IServiceProvider _serviceProvider;

    public IServiceProvider ServiceProvider
    {
        get => _serviceProvider ?? _httpContext?.RequestServices;
    }

    private Configs _configs;

    public Configs Configs
    {
        get => _configs;
    }

    private IRedisCacheService _redisCache;

    public IRedisCacheService RedisCache
    {
        get { return _redisCache; }
    }

    private IDistributedCache _cache;

    public IDistributedCache Cache
    {
        get { return _cache; }
    }

    private IMapper _mapper;

    public IMapper Mapper
    {
        get { return _mapper; }
    }


    public LoginUserInfo LoginUserInfo
    {
        get
        {
            if (!HttpUser.JwtToken.IsNullOrEmpty())
            {
                var onlineUser = AsyncHelper.RunSync(() =>
                    RedisCache.GetCacheAsync<LoginUserInfo>(
                        GlobalConstants.CacheKey.OnlineKey + HttpUser.JwtToken.ToMd5String16()));
                return onlineUser;
            }

            return null;
        }
    }

    private IHttpUser _httpUser;

    public IHttpUser HttpUser
    {
        get { return _httpUser; }
    }


    public ApeContext(IOptionsMonitor<Configs> configs, IMapper mapper, IHttpUser httpUser,
        IHttpContextAccessor httpContextAccessor = null,
        IRedisCacheService redisCache = null, IDistributedCache cache = null)
    {
        _configs = configs?.CurrentValue ?? new Configs();
        _httpContext = httpContextAccessor?.HttpContext;
        _cache = cache;
        _mapper = mapper;
        _httpUser = httpUser;
        _redisCache = redisCache;
    }
}
