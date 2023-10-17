using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.Vo;
using IP2Region.Net.XDB;
using Shyjus.BrowserDetection;

namespace ApeVolo.Business.Permission;

/// <summary>
/// 在线用户service
/// </summary>
public class OnlineUserService : IOnlineUserService
{
    #region 字段

    private readonly IBrowserDetector _browserDetector;
    private readonly ISearcher _ipSearcher;

    #endregion

    #region 构造函数

    public OnlineUserService(IBrowserDetector browserDetector, ISearcher searcher)
    {
        _browserDetector = browserDetector;
        _ipSearcher = searcher;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 保存在线用户
    /// </summary>
    /// <param name="jwtUserVo"></param>
    /// <param name="remoteIp"></param>
    public async Task<LoginUserInfo> SaveAsync(JwtUserVo jwtUserVo, string remoteIp)
    {
        var onlineUser = new LoginUserInfo
        {
            UserId = jwtUserVo.User.Id,
            Account = jwtUserVo.User.Username,
            NickName = jwtUserVo.User.NickName,
            DeptId = jwtUserVo.User.DeptId,
            DeptName = jwtUserVo.User.Dept.Name,
            Ip = remoteIp,
            Address = _ipSearcher.Search(remoteIp),
            OperatingSystem = _browserDetector.Browser?.OS,
            DeviceType = _browserDetector.Browser?.DeviceType,
            BrowserName = _browserDetector.Browser?.Name,
            Version = _browserDetector.Browser?.Version,
            LoginTime = DateTime.Now,
            CurrentPermission = new CurrentPermission
                { Roles = jwtUserVo.User.Authorizes, Urls = jwtUserVo.User.PermissionUrl }
        };
        return await Task.FromResult(onlineUser);
        // return await _redisCacheService.SetCacheAsync(GlobalConstants.CacheKey.OnlineKey + onlineUser.RemoteToken,
        //     onlineUser,
        //     TimeSpan.FromMinutes(30), RedisExpireType.Relative);
    }

    public async Task<JwtUserVo> FindJwtUserAsync(UserDto userDto)
    {
        var jwtUser = new JwtUserVo
        {
            User = userDto,
            DataScopes = new List<string>(),
            Roles = userDto.Authorizes
        };
        return await Task.FromResult(jwtUser);
    }

    #endregion
}
