using ApeVolo.Common.Global;
using Microsoft.AspNetCore.Authorization;

namespace ApeVolo.Api.Controllers.Base;

/// <summary>
/// API对外接口基控制器
/// </summary>
/* 鉴权方式
 * 1. [TokenFilter] 自定义过滤器
 * 2. [AllowAnonymous] 特性 不需要权限访问
 * 3. [Authorize(Roles = "Admin")] .net core 自带的角色授权 表示拥有Admin角色标识即可访问 不会进入AuthorizationHandler
 * 4. [ApeVoloAuthorize(new string[] { "admin", "common" })] 自定义的角色特性鉴权，表示拥有admin common角色标识即可访问
 * 5. [Authorize(Policy = GlobalSwitch.AuthPolicysName)] 自定义策略模式 重写AuthorizationHandler鉴权 配合第四点一起使用
 */
//[TokenFilter]
//[ApeVoloAuthorize(new string[] { "admin", "common" })]
[Authorize(Policy = AuthConstants.AuthPolicyName)]
public class BaseApiController : BaseController
{
}
