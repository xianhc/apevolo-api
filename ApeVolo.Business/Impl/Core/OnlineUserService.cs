using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Entity.Do.Other;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.Vo;

namespace ApeVolo.Business.Impl.Core;

/// <summary>
/// 在线用户service
/// </summary>
public class OnlineUserService : IOnlineUserService
{
    #region 字段

    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public OnlineUserService(IRedisCacheService redisCacheService)
    {
        _redisCacheService = redisCacheService;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 保存在线用户
    /// </summary>
    /// <param name="jwtUserVo"></param>
    /// <param name="token"></param>
    /// <param name="remoteIp"></param>
    public async Task<bool> SaveAsync(JwtUserVo jwtUserVo, string token, string remoteIp)
    {
        var onlineUser = new OnlineUser
        {
            UserId = jwtUserVo.User.Id,
            UserName = jwtUserVo.User.Username,
            NickName = jwtUserVo.User.NickName,
            Dept = jwtUserVo.User.Dept.Name,
            Browser = IpHelper.GetBrowserName(),
            Ip = remoteIp,
            Address = IpHelper.GetIpAddress(remoteIp),
            Key = token.ToMd5String16(),
            LoginTime = DateTime.Now,
            CurrentPermission = new CurrentPermission
                { Roles = jwtUserVo.User.Authorizes, Urls = jwtUserVo.User.PermissionUrl }
        };
        return await _redisCacheService.SetCacheAsync(RedisKey.OnlineKey + onlineUser.Key, onlineUser,
            TimeSpan.FromMinutes(30), RedisExpireType.Relative);
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