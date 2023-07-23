using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApeVolo.Api.Authentication.Jwt;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.Interface.Queued;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApeVolo.Api.Controllers.Auth;

/// <summary>
/// 授权管理
/// </summary>
[Area("Permission")]
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
            if (code.IsNullOrEmpty()) return Error(Localized.Get("CodeNotExist"));

            if (!code.Equals(authUser.Captcha)) return Error(Localized.Get("CodeWrong"));
        }

        var userDto = await _userService.QueryByNameAsync(authUser.Username);
        if (userDto == null) return Error(Localized.Get("{0}NotExist", Localized.Get("User")));
        var password = new RsaHelper(_apeContext.Configs.Rsa).Decrypt(authUser.Password);
        if (!userDto.Password.Equals(
                (password + userDto.SaltKey).ToHmacsha256String(_apeContext.Configs.HmacSecret)))
            return Error(Localized.Get("PasswrodWrong"));

        if (!userDto.Enabled) return Error(Localized.Get("{0}NotActivated", Localized.Get("User")));

        var netUser = await _userService.QueryByIdAsync(userDto.Id);

        //用户权限点 路由
        var permissionList = await _permissionService.QueryUserPermissionAsync(userDto.Id);
        netUser.Authorizes.AddRange(permissionList.Select(s => s.Permission).Where(s => !s.IsNullOrEmpty()));
        netUser.Authorizes.AddRange(netUser.Roles.Select(s => s.Permission));
        netUser.PermissionUrl.AddRange(permissionList.Select(s => s.LinkUrl).Where(s => !s.IsNullOrEmpty()));

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var jwtUserVo = await _onlineUserService.FindJwtUserAsync(netUser);
        var loginUserInfo = await _onlineUserService.SaveAsync(jwtUserVo, remoteIp);
        var token = await _tokenService.IssueTokenAsync(loginUserInfo);
        loginUserInfo.AccessToken = token.AccessToken;
        var onlineKey = loginUserInfo.AccessToken.ToMd5String16();
        await _apeContext.Cache.SetAsync(
            GlobalConstants.CacheKey.OnlineKey + onlineKey,
            loginUserInfo, TimeSpan.FromHours(3), CacheExpireType.Relative);
        var dic = new Dictionary<string, object>
            { { "user", jwtUserVo }, { "token", token.TokenType + " " + token.AccessToken } };

        return dic.ToJson();
    }


    [HttpGet]
    [Route("/auth/info")]
    [Description("UserInfo")]
    [ApeVoloOnline]
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
    [Description("GetCode")]
    [Route("/auth/captcha")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Captcha()
    {
        //在linux平台下会报异常 The type initializer for ‘Gdip‘ threw an exception
        //可安装libgdiplus包重启服务器解决或使用下面的SixLabors.ImageSharp库支持跨平台
        //var (imgBytes, code) = ImgVerifyCodeHelper.BuildVerifyCode();
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
    [Description("GetCode")]
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
    [Description("LoginOut")]
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
}
