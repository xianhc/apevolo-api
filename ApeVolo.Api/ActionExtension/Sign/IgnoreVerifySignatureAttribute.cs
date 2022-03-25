using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApeVolo.Api.ActionExtension.Sign;

public class IgnoreVerifySignatureAttribute : BaseActionFilter
{
    /// <summary>
    /// Action执行之前执行
    /// </summary>
    /// <param name="filterContext"></param>
    public override async Task OnActionExecuting(ActionExecutingContext filterContext)
    {
        await Task.CompletedTask;
    }
}