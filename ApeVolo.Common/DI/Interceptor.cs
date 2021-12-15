using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace ApeVolo.Common.DI
{
    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //如果方法使用多个拦截属性 需要使用循环方式  这里只使用一个FirstOrDefault();
            var allFilter = invocation.MethodInvocationTarget.GetCustomAttributes<BaseFilterAttribute>(true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes<BaseFilterAttribute>(true))
                .Where(x => x is IFilter)
                .Select(x => (IFilter) x)
                .ToList().FirstOrDefault();

            //执行前
            //执行
            invocation.Proceed();

            //执行后
            //allFilter.ForEach(aFiler =>
            //{
            //    aFiler.OnActionExecuted(invocation);
            //});

            if (allFilter != null)
            {
                allFilter.OnActionExecuted(invocation);
            }
        }
    }
}