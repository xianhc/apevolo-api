using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Repository.UnitOfWork;
using Castle.DynamicProxy;

namespace ApeVolo.Api.Aop;

/// <summary>
/// 事务拦截器
/// </summary>
public class TransactionAop : IInterceptor
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionAop(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 实例化IInterceptor唯一方法 
    /// </summary>
    /// <param name="invocation">包含被拦截方法的信息</param>
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        //对当前方法的特性验证
        //如果需要验证
        if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(UseTranAttribute)) is
            UseTranAttribute)
        {
            try
            {
                _unitOfWork.BeginTran();

                invocation.Proceed();


                // 异步获取，先执行
                if (IsAsyncMethod(invocation.Method))
                {
                    var result = invocation.ReturnValue;
                    if (result is Task)
                    {
                        Task.WaitAll(result as Task);
                    }
                }

                _unitOfWork.CommitTran();
            }
            catch
            {
                _unitOfWork.RollbackTran();
            }
        }
        else
        {
            invocation.Proceed();
        }
    }

    private static bool IsAsyncMethod(MethodInfo method)
    {
        return
            method.ReturnType == typeof(Task) ||
            method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
            ;
    }
}