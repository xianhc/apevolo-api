using System.Threading.Tasks;
using ApeVolo.Common.WebApp;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Vo;

namespace ApeVolo.IBusiness.Interface.Permission;

/// <summary>
/// 在线用户接口
/// </summary>
public interface IOnlineUserService
{
    #region 基础接口

    /// <summary>
    /// 保存在线用户
    /// </summary>
    /// <param name="jwtUserVo"></param>
    /// <param name="remoteIp"></param>
    Task<LoginUserInfo> SaveAsync(JwtUserVo jwtUserVo, string remoteIp);

    /// <summary>
    /// jwt用户信息
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    Task<JwtUserVo> FindJwtUserAsync(UserDto userDto);

    #endregion
}
