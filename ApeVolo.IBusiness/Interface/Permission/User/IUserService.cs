using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.Permission.User;
using ApeVolo.IBusiness.ExportModel;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;

namespace ApeVolo.IBusiness.Interface.Permission.User;

/// <summary>
/// 用户接口
/// </summary>
public interface IUserService : IBaseServices<Entity.Permission.User.User>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateUserDto createUpdateUserDto);
    Task<bool> UpdateAsync(CreateUpdateUserDto createUpdateUserDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<UserDto>> QueryAsync(UserQueryCriteria userQueryCriteria, Pagination pagination);

    Task<List<ExportBase>> DownloadAsync(UserQueryCriteria userQueryCriteria);

    #endregion

    #region 扩展接口

    /// <summary>
    /// 查找用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<UserDto> QueryByIdAsync(long userId);

    /// <summary>
    /// 查找用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <returns>用户实体</returns>
    Task<UserDto> QueryByNameAsync(string userName);
    // /// <summary>
    // /// 用户岗位列表
    // /// </summary>
    // /// <param name="userId"></param>
    // /// <returns></returns>
    // Task<List<JobSmallDto>> GetJobListAsync(string userId);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<List<UserDto>> QueryByRoleIdAsync(long roleId);

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    /// <param name="deptIds"></param>
    /// <returns></returns>
    Task<List<UserDto>> QueryByDeptIdsAsync(List<long> deptIds);

    /// <summary>
    /// 修改个人中心信息
    /// </summary>
    /// <param name="updateUserCenterDto"></param>
    /// <returns></returns>
    Task<bool> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userPassDto"></param>
    /// <returns></returns>
    Task<bool> UpdatePasswordAsync(UpdateUserPassDto userPassDto);


    /// <summary>
    /// 修改邮箱
    /// </summary>
    /// <param name="updateUserEmailDto"></param>
    /// <returns></returns>
    Task<bool> UpdateEmailAsync(UpdateUserEmailDto updateUserEmailDto);

    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    Task<bool> UpdateAvatarAsync(IFormFile file);

    #endregion
}