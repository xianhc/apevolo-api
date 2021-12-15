using System;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using Castle.DynamicProxy;

namespace ApeVolo.Api.Aop
{
    /// <summary>
    /// Redis拦截器 暂只支持一个参数,
    /// 需要多个参数的可以自行扩展，获取所有参数拼起来即可
    /// </summary>
    public class RedisAop : IInterceptor
    {
        private readonly IRedisCacheService _redisCacheService;

        public RedisAop(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            if (method.ReturnType == typeof(void) || method.ReturnType == typeof(Task))
            {
                invocation.Proceed();
                return;
            }


            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(RedisCachingAttribute)) is
                RedisCachingAttribute redisCachingAttribute)
            {
                var cacheKey = CreateCacheKey(invocation, redisCachingAttribute);
                var cacheValue = AsyncHelper.RunSync(() => _redisCacheService.GetCacheStrAsync<string>(cacheKey));
                if (cacheValue != null)
                {
                    Type returnType;
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();
                    }
                    else
                    {
                        returnType = method.ReturnType;
                    }

                    dynamic result = cacheValue.ToObject(returnType);
                    invocation.ReturnValue = typeof(Task).IsAssignableFrom(method.ReturnType)
                        ? Task.FromResult(result)
                        : result;
                    return;
                }

                invocation.Proceed();

                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;

                    //Type type = invocation.ReturnValue?.GetType();
                    var type = invocation.Method.ReturnType;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }

                    response ??= string.Empty;

                    AsyncHelper.RunSync(() => _redisCacheService.SetCacheAsync(cacheKey, response,
                        TimeSpan.FromMinutes(redisCachingAttribute.Expiration)));
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
        /// <param name="redisCachingAttribute"></param>
        /// <returns></returns>
        private static string CreateCacheKey(IInvocation invocation, RedisCachingAttribute redisCachingAttribute)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = GetArgumentValue(invocation.Arguments.ToList()[0].ToString());

            string key = "";
            if (redisCachingAttribute.KeyPrefix.IsNullOrEmpty())
            {
                key = $"{typeName}:{methodName}:";
            }
            else
            {
                key = redisCachingAttribute.KeyPrefix;
            }

            return key + methodArguments;
        }

        /// <summary>
        /// 获取参数值类型值
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static string GetArgumentValue(object arg)
        {
            if (arg is DateTime time)
                return time.ToString("yyyyMMddHHmmss").ToMd5String();

            if (arg.IsNull() || arg is string or ValueType)
                return arg.ToString().ToMd5String();
            return string.Empty;
        }
    }
}