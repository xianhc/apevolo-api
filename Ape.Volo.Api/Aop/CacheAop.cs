using System;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.Extention;
using Castle.DynamicProxy;

namespace Ape.Volo.Api.Aop;

/// <summary>
/// Redis缓存拦截器
/// </summary>
public class CacheAop : IInterceptor
{
    private readonly ICache _cache;

    public CacheAop(ICache cache)
    {
        _cache = cache;
    }

    public void Intercept(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        if (method.ReturnType == typeof(void) || method.ReturnType == typeof(Task))
        {
            invocation.Proceed();
            return;
        }


        if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(UseCacheAttribute)) is
            UseCacheAttribute useCacheAttribute)
        {
            var cacheKey = CreateCacheKey(invocation, useCacheAttribute);
            //var cacheValue = AsyncHelper.RunSync(() => _redisCacheService.GetAsync<string>(cacheKey));

            var cacheValue = _cache.Get<dynamic>(cacheKey);
            if (cacheValue != null)
            {
                // Type  returnType = typeof(Task).IsAssignableFrom(method.ReturnType)
                //     ? method.ReturnType.GenericTypeArguments.FirstOrDefault()
                //     : method.ReturnType;
                // dynamic result = cacheValue.ToObject(returnType);
                dynamic result = cacheValue;
                invocation.ReturnValue = typeof(Task).IsAssignableFrom(method.ReturnType)
                    ? Task.FromResult(result)
                    : result;
                return;
            }


            invocation.Proceed();

            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                object response = null;

                //Type type = invocation.ReturnValue?.GetType();
                var type = invocation.Method.ReturnType;
                if (typeof(Task).IsAssignableFrom(type))
                {
                    var resultProperty = type.GetProperty("Result");
                    if (resultProperty != null) response = resultProperty.GetValue(invocation.ReturnValue);
                }
                else
                {
                    response = invocation.ReturnValue;
                }

                //response ??= string.Empty;
                if (response.IsNullOrEmpty())
                {
                    return;
                }

                // AsyncHelper.RunSync(() => _redisCacheService.SetAsync(cacheKey, response,
                //     TimeSpan.FromMinutes(redisCachingAttribute.Expiration), null));
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(useCacheAttribute.Expiration),
                    null);
            }
        }
        else
        {
            invocation.Proceed();
        }
    }

    /// <summary>
    /// 构建Redis Key
    /// </summary>
    /// <param name="invocation"></param>
    /// <param name="useCacheAttribute"></param>
    /// <returns></returns>
    private static string CreateCacheKey(IInvocation invocation, UseCacheAttribute useCacheAttribute)
    {
        var typeName = invocation.TargetType.Name;
        var methodName = invocation.Method.Name;

        //支持多参数包括实体类，建议不要超过三个， 避免产生的redis key过长
        var methodArguments = invocation.Arguments.Select(GetArgumentValue).ToList();

        var key = useCacheAttribute.KeyPrefix.IsNullOrEmpty()
            ? $"{typeName}:{methodName}:"
            : useCacheAttribute.KeyPrefix;

        methodArguments.ForEach(arg => { key = $"{key}{arg}:"; });

        return key.TrimEnd(':');
    }

    /// <summary>
    /// 获取参数值类型值
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private static string GetArgumentValue(object arg)
    {
        if (arg == null) return string.Empty;

        switch (arg)
        {
            case string:
            case long:
                return arg.ToString().ToMd5String16();
            case DateTime:
                return arg.ToString("yyyyMMddHHmmss").ToMd5String16();
            default:
            {
                if (arg.GetType().IsClass)
                {
                    return arg.ToJson().ToMd5String16();
                }

                return string.Empty;
            }
        }
    }
}
