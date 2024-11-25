using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ape.Volo.Api.Authentication.Jwt;
using Ape.Volo.Api.Controllers.Base;
using Ape.Volo.Common;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Common.Caches;
using Ape.Volo.Common.ConfigOptions;
using Ape.Volo.Common.Exception;
using Ape.Volo.Common.Extensions;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Helper;
using Ape.Volo.Common.SnowflakeIdHelper;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Interface.Queued;
using Ape.Volo.IBusiness.Interface.System;
using Ape.Volo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ape.Volo.Api.Controllers.Auth;

/// <summary>
/// 授权管理
/// </summary>
[Area("授权管理")]
[Route("/auth")]
public class AuthorizationController : BaseApiController
{
    #region 构造函数

    public AuthorizationController(IUserService userService, IPermissionService permissionService,
        IOnlineUserService onlineUserService, IQueuedEmailService queuedEmailService,
        ITokenService tokenService, ITokenBlacklistService tokenBlacklistService)
    {
        _userService = userService;
        _permissionService = permissionService;
        _onlineUserService = onlineUserService;
        _queuedEmailService = queuedEmailService;
        _tokenService = tokenService;
        _tokenBlacklistService = tokenBlacklistService;
    }

    #endregion


    #region 字段

    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;
    private readonly IOnlineUserService _onlineUserService;
    private readonly IQueuedEmailService _queuedEmailService;
    private readonly ITokenService _tokenService;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    #endregion

    #region 内部接口

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Description("获取验证码")]
    [Route("captcha")]
    [AllowAnonymous]
    [NotAudit]
    public async Task<ActionResult> Captcha()
    {
        var showCaptcha = true; //是否显示验证码
        var captchaOptions = App.GetOptions<CaptchaOptions>();
        if (captchaOptions.Threshold > 0)
        {
            var thresholdCacheKey = GlobalConstants.CachePrefix.Threshold + App.HttpContext.Connection.RemoteIpAddress;
            var failedThreshold = await App.Cache.GetAsync<int>(thresholdCacheKey);
            if (failedThreshold <= 0)
            {
                failedThreshold = 1;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut), null);
            }

            showCaptcha = failedThreshold > captchaOptions.Threshold;
        }


        var (imgBytes, code) = SixLaborsImageHelper.BuildVerifyCode(captchaOptions.ImgWidth, captchaOptions.ImgHeight,
            captchaOptions.FontSize, captchaOptions.KeyLength);
        var img = ImgHelper.ToBase64StringUrl(imgBytes);
        var captchaId = GlobalConstants.CachePrefix.CaptchaId +
                        IdHelper.GetId().Base64Encode();
        await App.Cache.SetAsync(captchaId, code, TimeSpan.FromMinutes(2), null);
        var response = new
        {
            img,
            captchaId,
            showCaptcha
        };

        return Ok(response);
    }


    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="authUser"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("login")]
    [Description("用户登录")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginAuthUser authUser)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }


        var loginFailedLimitOptions = App.GetOptions<LoginFailedLimitOptions>();
        var attempsCacheKey = GlobalConstants.CachePrefix.Attempts + App.HttpContext.Connection.RemoteIpAddress +
                              authUser.Username;
        LoginAttempt loginAttempt = null;
        if (loginFailedLimitOptions.Enabled)
        {
            loginAttempt = await App.Cache.GetAsync<LoginAttempt>(attempsCacheKey);
            if (loginAttempt.IsNull())
            {
                loginAttempt = new LoginAttempt
                {
                    Count = 0, IsLocked = false, LockUntil = DateTime.MinValue
                };
                await App.Cache.SetAsync(attempsCacheKey, loginAttempt,
                    TimeSpan.FromSeconds(loginFailedLimitOptions.Lockout), null);
            }

            if (loginAttempt.IsLocked && DateTime.Now < loginAttempt.LockUntil)
            {
                // 可以实施账户锁定时，通过邮件或短信通知用户。
                // 可以实施账户锁定后要求管理员手动解锁
                return Error($"账户已锁定，请稍后重试。解锁时间：{loginAttempt.LockUntil:yyyy-MM-dd HH:mm:ss}");
            }
        }


        var captchaOptions = App.GetOptions<CaptchaOptions>();
        var showCaptcha = true; //是否显示验证码
        var thresholdCacheKey = GlobalConstants.CachePrefix.Threshold + App.HttpContext.Connection.RemoteIpAddress;
        var failedThreshold = 0;
        if (captchaOptions.Threshold > 0)
        {
            failedThreshold = await App.Cache.GetAsync<int>(thresholdCacheKey);
            if (failedThreshold <= 0)
            {
                failedThreshold = 1;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut), null);
            }

            showCaptcha = failedThreshold > captchaOptions.Threshold;
        }


        await App.Cache.RemoveAsync(authUser.CaptchaId);
        if (!App.GetOptions<SystemOptions>().IsQuickDebug && showCaptcha)
        {
            var code = await App.Cache.GetAsync<string>(authUser.CaptchaId);

            if (!code.Equals(authUser.Captcha))
            {
                if (captchaOptions.Threshold > 0)
                {
                    failedThreshold++;
                    await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                        TimeSpan.FromSeconds(captchaOptions.TimeOut),
                        null);
                }

                return Error("验证码输入错误!");
            }
        }

        var userDto = await _userService.QueryByNameAsync(authUser.Username);
        if (userDto == null)
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

            return Error("用户不存在");
        }

        var password = new RsaHelper(App.GetOptions<RsaOptions>()).Decrypt(authUser.Password);
        if (!BCryptHelper.Verify(password, userDto.Password))
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

            if (loginFailedLimitOptions.Enabled && loginAttempt != null)
            {
                loginAttempt.Count++;
                if (loginAttempt.Count >= loginFailedLimitOptions.MaxAttempts)
                {
                    loginAttempt.IsLocked = true;
                    loginAttempt.LockUntil = DateTime.Now.AddSeconds(loginFailedLimitOptions.Lockout);
                }


                await App.Cache.SetAsync(attempsCacheKey, loginAttempt,
                    TimeSpan.FromSeconds(loginFailedLimitOptions.Lockout), null);
            }

            return loginFailedLimitOptions.Enabled ? Error("密码错误,连续输入三次错误将锁定账号！") : Error("密码错误");
        }

        if (!userDto.Enabled)
        {
            if (captchaOptions.Threshold > 0)
            {
                failedThreshold++;
                await App.Cache.SetAsync(thresholdCacheKey, failedThreshold,
                    TimeSpan.FromSeconds(captchaOptions.TimeOut),
                    null);
            }

            return Error("用户未激活");
        }

        await App.Cache.RemoveAsync(thresholdCacheKey);
        await App.Cache.RemoveAsync(attempsCacheKey);
        var netUser = await _userService.QueryByIdAsync(userDto.Id);
        return await LoginResult(netUser, "login");
    }


    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("refreshToken")]
    [Description("刷新Token")]
    [AllowAnonymous]
    [NotAudit]
    public async Task<ActionResult> RefreshToken(string token = "")
    {
        if (token.IsNullOrEmpty()) return Error("token已丢失，请重新登录！");

        var tokenMd5 = token.ToMd5String16();
        var tokenBlacklist = await _tokenBlacklistService.TableWhere(x => x.AccessToken == tokenMd5, null, null, true)
            .FirstAsync();
        if (tokenBlacklist.IsNull())
        {
            var jwtSecurityToken = await _tokenService.ReadJwtToken(token);
            if (jwtSecurityToken != null)
            {
                var userId = Convert.ToInt64(jwtSecurityToken.Claims
                    .FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Jti)?.Value);
                var loginTime = Convert.ToInt64(jwtSecurityToken.Claims
                    .FirstOrDefault(s => s.Type == AuthConstants.JwtClaimTypes.Iat)?.Value).TicksToDateTime();
                var nowTime = DateTime.Now.ToLocalTime();
                var refreshTime = loginTime.AddHours(App.GetOptions<JwtAuthOptions>().RefreshTokenExpires);
                // 允许token刷新时间内
                if (nowTime <= refreshTime)
                {
                    var netUser = await _userService.QueryByIdAsync(userId);
                    if (netUser.IsNotNull())
                        if (netUser.UpdateTime == null || netUser.UpdateTime < loginTime)
                            return await LoginResult(netUser, "refresh");
                }
            }
        }

        return Error("token验证失败，请重新登录！");
    }


    [HttpGet]
    [Route("info")]
    [Description("个人信息")]
    [NotAudit]
    public async Task<ActionResult> GetInfo()
    {
        var netUser = await _userService.QueryByIdAsync(App.HttpUser.Id);
        var permissionIdentifierList = await _permissionService.GetPermissionIdentifierAsync(netUser.Id);
        permissionIdentifierList.AddRange(netUser.Roles.Select(r => r.Permission));
        var jwtUserVo = await _onlineUserService.CreateJwtUserAsync(netUser, permissionIdentifierList);
        return Ok(jwtUserVo);
    }


    /// <summary>
    /// 获取验证码，申请变更邮箱
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Description("获取邮箱验证码")]
    [Route("code/reset/email")]
    public async Task<ActionResult> ResetEmail(string email)
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
    [Route("logout")]
    [Description("用户登出")]
    [AllowAnonymous]
    public async Task<ActionResult> Logout()
    {
        //清理缓存
        if (App.HttpUser.IsNotNull())
        {
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.OnlineKey +
                                        App.HttpUser.JwtToken.ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserInfoById +
                                        App.HttpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserMenuById +
                                        App.HttpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserPermissionRoles +
                                        App.HttpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserPermissionUrls +
                                        App.HttpUser.Id.ToString().ToMd5String16());
            await App.Cache.RemoveAsync(GlobalConstants.CachePrefix.UserDataScopeById +
                                        App.HttpUser.Id.ToString().ToMd5String16());
        }

        return Success();
    }


    /// <summary>
    /// swagger登录
    /// </summary>
    /// <param name="swaggerLoginAuthUser"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("swagger/login")]
    [Description("用户登录")]
    [AllowAnonymous]
    public async Task<ActionResult> SwaggerLogin([FromBody] SwaggerLoginAuthUser swaggerLoginAuthUser)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var userDto = await _userService.QueryByNameAsync(swaggerLoginAuthUser.Username);
        if (userDto == null) return Error("用户不存在");
        var password = new RsaHelper(App.GetOptions<RsaOptions>()).Decrypt(swaggerLoginAuthUser.Password);
        if (!BCryptHelper.Verify(password, userDto.Password))
            return Error("密码错误");

        if (!userDto.Enabled) return Error("用户未激活");
        App.HttpContext.Session.SetInt32("swagger-key", 1);
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
    private async Task<ActionResult> LoginResult(UserDto userDto, string type)
    {
        var permissionIdentifierList = new List<string>();
        var refresh = true;
        if (type.Equals("login"))
        {
            refresh = false;
            permissionIdentifierList = await _permissionService.GetPermissionIdentifierAsync(userDto.Id);
            permissionIdentifierList.AddRange(userDto.Roles.Select(r => r.Permission));
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var jwtUserVo = await _onlineUserService.CreateJwtUserAsync(userDto, permissionIdentifierList);
        var loginUserInfo = await _onlineUserService.SaveLoginUserAsync(jwtUserVo, remoteIp);
        var token = await _tokenService.IssueTokenAsync(loginUserInfo, refresh);
        loginUserInfo.AccessToken = refresh ? token.RefreshToken : token.AccessToken;
        var onlineKey = loginUserInfo.AccessToken.ToMd5String16();
        await App.Cache.SetAsync(
            GlobalConstants.CachePrefix.OnlineKey + onlineKey,
            loginUserInfo, TimeSpan.FromHours(2), CacheExpireType.Absolute);

        switch (type)
        {
            case "login":
                var response = new
                {
                    user = jwtUserVo, token
                };
                return Ok(response);
            case "refresh":
                return Ok(token);
            default:
                return Ok();
        }
    }

    #endregion
}
