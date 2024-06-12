using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ape.Volo.Common.WebApp;
using Ape.Volo.IBusiness.Dto.Permission;
using Ape.Volo.IBusiness.Interface.Permission;
using Ape.Volo.IBusiness.Vo;
using IP2Region.Net.Abstractions;
using Shyjus.BrowserDetection;

namespace Ape.Volo.Business.Permission;

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
    public async Task<LoginUserInfo> SaveLoginUserAsync(JwtUserVo jwtUserVo, string remoteIp)
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
            IsAdmin = jwtUserVo.User.IsAdmin,
            TenantId = jwtUserVo.User.TenantId
        };
        return await Task.FromResult(onlineUser);
    }

    public async Task<JwtUserVo> CreateJwtUserAsync(UserDto userDto, List<string> permissionRoles)
    {
        var jwtUser = new JwtUserVo
        {
            User = userDto,
            DataScopes = new List<string>(),
            Roles = permissionRoles
        };
        return await Task.FromResult(jwtUser);
    }

    #endregion
}
