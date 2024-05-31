using System;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Ape.Volo.Common.WebApp;

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

    private ICache _cache;

    public ICache Cache
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
                // var loginUserInfo = AsyncHelper.RunSync(() =>
                //     RedisCache.GetAsync<LoginUserInfo>(
                //         GlobalConstants.CacheKey.OnlineKey + HttpUser.JwtToken.ToMd5String16()));
                var loginUserInfo = Cache.Get<LoginUserInfo>(
                    GlobalConstants.CachePrefix.OnlineKey + HttpUser.JwtToken.ToMd5String16());
                return loginUserInfo;
            }

            return null;
        }
    }

    private IHttpUser _httpUser;

    public IHttpUser HttpUser
    {
        get { return _httpUser; }
    }


    public ApeContext(IOptionsMonitor<Configs> configs, IHttpContextAccessor httpContextAccessor,
        IMapper mapper, IHttpUser httpUser, ICache cache)
    {
        _configs = configs?.CurrentValue ?? new Configs();
        _httpContext = httpContextAccessor?.HttpContext;
        _cache = cache;
        _mapper = mapper;
        _httpUser = httpUser;
    }
}
