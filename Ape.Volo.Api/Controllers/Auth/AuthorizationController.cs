using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Api.Authentication.Jwt;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extention;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Interface.Queued;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ape.Volo.Api.Controllers.Auth;

/// <summary>
/// 授权管理
/// </summary>
[Area("授权管理")]
[Route("[controller]/[action]")]
public class AuthorizationController : BaseApiController
{
    #region 字段

    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;
    private readonly IOnlineUserService _onlineUserService;
    private readonly IQueuedEmailService _queuedEmailService;
    private readonly ApeContext _apeContext;
    private readonly ITokenService _tokenService;

    #endregion

    #region 构造函数

    public AuthorizationController(IUserService userService, IPermissionService permissionService,
        IOnlineUserService onlineUserService, IQueuedEmailService queuedEmailService, ApeContext apeContext,
        ITokenService tokenService)
    {
        _userService = userService;
        _permissionService = permissionService;
        _onlineUserService = onlineUserService;
        _queuedEmailService = queuedEmailService;
        _apeContext = apeContext;
        _tokenService = tokenService;
    }

    #endregion

    #region 内部接口

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="authUser"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/auth/login")]
    [Description("用户登录")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login([FromBody] LoginAuthUser authUser)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var code = await _apeContext.Cache.GetAsync<string>(authUser.CaptchaId);
        await _apeContext.Cache.RemoveAsync(authUser.CaptchaId);
        if (!_apeContext.Configs.IsQuickDebug)
        {
            if (code.IsNullOrEmpty()) return Error("验证码不存在或已过期");

            if (!code.Equals(authUser.Captcha)) return Error("验证码错误");
        }

        var userDto = await _userService.QueryByNameAsync(authUser.Username);
        if (userDto == null) return Error("用户不存在");
        var password = new RsaHelper(_apeContext.Configs.Rsa).Decrypt(authUser.Password);
        if (!BCryptHelper.Verify(password, userDto.Password))
            return Error("密码错误");

        if (!userDto.Enabled) return Error("用户未激活");

        var netUser = await _userService.QueryByIdAsync(userDto.Id);
        if (netUser != null)
        {
            return await LoginResult(netUser, "login");
        }

        return Error();
    }


    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("/auth/refreshToken")]
    [Description("刷新Token")]
    [AllowAnonymous]
    [NotAudit]
    public async Task<ActionResult<object>> RefreshToken(string token = "")
    {
        if (token.IsNullOrEmpty())
        {
            return Error("token已丢弃，请重新登录！");
        }

        var jwtSecurityToken = await _tokenService.ReadJwtToken(token);
        if (jwtSecurityToken != null)
        {
            var userId = Convert.ToInt64(jwtSecurityToken.Claims
                .FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Jti)?.Value);
            var loginTime = Convert.ToInt64(jwtSecurityToken.Claims
                .FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Iat)?.Value).TicksToDateTime();
            var nowTime = DateTime.Now.ToLocalTime();
            var refreshTime = loginTime.AddSeconds(_apeContext.Configs.JwtAuthOptions.RefreshTokenExpires);
            // 允许token刷新时间内
            if (nowTime <= refreshTime)
            {
                var netUser = await _userService.QueryByIdAsync(userId);
                if (netUser.IsNotNull())
                {
                    if (netUser.UpdateTime == null || netUser.UpdateTime < loginTime)
                    {
                        return await LoginResult(netUser, "refresh");
                    }
                }
            }
        }

        return Error("token验证失败，请重新登录！");
    }


    [HttpGet]
    [Route("/auth/info")]
    [Description("个人信息")]
    [ApeVoloOnline]
    [NotAudit]
    public async Task<ActionResult<object>> GetInfo()
    {
        var netUser = await _userService.QueryByIdAsync(_apeContext.HttpUser.Id);
        //用户权限点 路由
        var permissionList = await _permissionService.QueryUserPermissionAsync(_apeContext.HttpUser.Id);
        netUser.Authorizes.AddRange(permissionList.Select(s => s.Permission).Where(s => !s.IsNullOrEmpty()));
        netUser.Authorizes.AddRange(netUser.Roles.Select(r => r.Permission));
        var jwtUserVo = await _onlineUserService.FindJwtUserAsync(netUser);
        return jwtUserVo.ToJson();
    }

    /// <summary>
    /// 获取图片验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Description("获取验证码")]
    [Route("/auth/captcha")]
    [AllowAnonymous]
    [NotAudit]
    public async Task<ActionResult<object>> Captcha()
    {
        var (imgBytes, code) = SixLaborsImageHelper.BuildVerifyCode();
        var imgUrl = ImgHelper.ToBase64StringUrl(imgBytes);
        var captchaId = GlobalConstants.CacheKey.CaptchaId + GuidHelper.GenerateKey();
        await _apeContext.Cache.SetAsync(captchaId, code, TimeSpan.FromMinutes(2), null);
        var dic = new Dictionary<string, string> { { "img", imgUrl }, { "captchaId", captchaId } };
        return dic.ToJson();
    }

    /// <summary>
    /// 获取验证码，申请变更邮箱
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Description("获取邮箱验证码")]
    [Route("/auth/code/reset/email")]
    [ApeVoloOnline]
    public async Task<ActionResult<object>> ResetEmail(string email)
    {
        if (!email.IsEmail()) throw new BadRequestException("请输入正确的邮箱！");

        var isTrue = await _queuedEmailService.ResetEmail(email, "EmailVerificationCode");
        return isTrue ? Success() : Error();
    }


    /// <summary>
    /// 系统用户登出
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("/auth/logout")]
    [Description("用户登出")]
    //[ApeVoloOnline]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Logout()
    {
        //清理缓存
        if (!_apeContext.HttpUser.IsNotNull()) return Success();
        await _apeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.OnlineKey +
                                            _apeContext.HttpUser.JwtToken.ToMd5String16());
        await _apeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoById +
                                            _apeContext.HttpUser.Id.ToString().ToMd5String16());
        await _apeContext.Cache.RemoveAsync(
            GlobalConstants.CacheKey.UserInfoByName + _apeContext.HttpUser.Account.ToMd5String16());

        return Success();
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 登录或刷新token相应结果
    /// </summary>
    /// <param name="userDto"></param>
    /// <param name="type">login:登录,refresh:刷新token</param>
    /// <returns></returns>
    private async Task<string> LoginResult(UserDto userDto, string type)
    {
        //用户权限点 路由
        var permissionList = await _permissionService.QueryUserPermissionAsync(userDto.Id);
        userDto.Authorizes.AddRange(permissionList.Select(s => s.Permission).Where(s => !s.IsNullOrEmpty()));
        userDto.Authorizes.AddRange(userDto.Roles.Select(s => s.Permission));
        userDto.PermissionUrl.AddRange(permissionList.Select(s => s.LinkUrl).Where(s => !s.IsNullOrEmpty()));

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var jwtUserVo = await _onlineUserService.FindJwtUserAsync(userDto);
        var loginUserInfo = await _onlineUserService.SaveAsync(jwtUserVo, remoteIp);
        var token = await _tokenService.IssueTokenAsync(loginUserInfo);
        loginUserInfo.AccessToken = token.AccessToken;
        var onlineKey = loginUserInfo.AccessToken.ToMd5String16();
        await _apeContext.Cache.SetAsync(
            GlobalConstants.CacheKey.OnlineKey + onlineKey,
            loginUserInfo, TimeSpan.FromHours(2), CacheExpireType.Absolute);

        switch (type)
        {
            case "login":
                var dic = new Dictionary<string, object>
                    { { "user", jwtUserVo }, { "token", token } };
                return dic.ToJson();
            case "refresh":
                return token.ToJson();
            default:
                return "";
        }
    }

    #endregion
}
