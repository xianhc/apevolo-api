using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.ExportModel.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace ApeVolo.Business.Permission;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseServices<User>, IUserService
{
    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;
    private readonly IJobService _jobService;
    private readonly IDataScopeService _dataScopeService;

    #endregion

    #region 构造函数

    public UserService(IDepartmentService departmentService, ApeContext apeContext,
        IRoleService roleService, IJobService jobService, IDataScopeService dataScopeService) : base(apeContext)
    {
        _departmentService = departmentService;
        _roleService = roleService;
        _jobService = jobService;
        _dataScopeService = dataScopeService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        if (await TableWhere(x => x.Username == createUpdateUserDto.Username).AnyAsync())
        {
            throw new BadRequestException($"名称=>{createUpdateUserDto.Username}=>已存在!");
        }

        if (await TableWhere(x => x.Email == createUpdateUserDto.Email).AnyAsync())
        {
            throw new BadRequestException($"邮箱=>{createUpdateUserDto.Email}=>已存在!");
        }

        if (await TableWhere(x => x.Phone == createUpdateUserDto.Phone).AnyAsync())
        {
            throw new BadRequestException($"电话=>{createUpdateUserDto.Phone}=>已存在!");
        }

        var user = ApeContext.Mapper.Map<User>(createUpdateUserDto);

        //设置用户密码
        user.Password = BCryptHelper.Hash("123456");
        user.DeptId = user.Dept.Id;
        //用户
        await AddEntityAsync(user);

        //角色
        if (user.Roles.Count < 1)
        {
            throw new BadRequestException("角色至少选择一个");
        }

        await SugarClient.Deleteable<UserRoles>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userRoles = new List<UserRoles>();
        userRoles.AddRange(user.Roles.Select(x => new UserRoles() { UserId = user.Id, RoleId = x.Id }));
        await SugarClient.Insertable(userRoles).ExecuteCommandAsync();

        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException("岗位至少选择一个");
        }


        await SugarClient.Deleteable<UserJobs>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userJobs = new List<UserJobs>();
        userJobs.AddRange(user.Jobs.Select(x => new UserJobs() { UserId = user.Id, JobId = x.Id }));
        await SugarClient.Insertable(userJobs).ExecuteCommandAsync();

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        //取出待更新数据
        var oldUser = await TableWhere(x => x.Id == createUpdateUserDto.Id).FirstAsync();
        if (oldUser.IsNull())
        {
            throw new BadRequestException("数据不存在！");
        }

        if (oldUser.Username != createUpdateUserDto.Username &&
            await TableWhere(x => x.Username == createUpdateUserDto.Username).AnyAsync())
        {
            throw new BadRequestException($"名称=>{createUpdateUserDto.Username}=>已存在!");
        }

        if (oldUser.Email != createUpdateUserDto.Email &&
            await TableWhere(x => x.Email == createUpdateUserDto.Email).AnyAsync())
        {
            throw new BadRequestException($"邮箱=>{createUpdateUserDto.Email}=>已存在!");
        }

        if (oldUser.Phone != createUpdateUserDto.Phone &&
            await TableWhere(x => x.Phone == createUpdateUserDto.Phone).AnyAsync())
        {
            throw new BadRequestException($"电话=>{createUpdateUserDto.Phone}=>已存在!");
        }

        //验证角色等级
        var levels = (await _roleService.QueryByUserIdAsync(oldUser.Id)).Select(x => x.Level);
        await _roleService.VerificationUserRoleLevelAsync(levels.Min());
        var user = ApeContext.Mapper.Map<User>(createUpdateUserDto);
        user.DeptId = user.Dept.Id;
        //更新用户
        await UpdateEntityAsync(user, new List<string> { "password", "salt_key", "avatar_name", "avatar_path" });
        //角色
        if (user.Roles.Count < 1)
        {
            throw new BadRequestException("角色至少选择一个！");
        }

        await SugarClient.Deleteable<UserRoles>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userRoles = new List<UserRoles>();
        userRoles.AddRange(user.Roles.Select(x => new UserRoles() { UserId = user.Id, RoleId = x.Id }));
        await SugarClient.Insertable(userRoles).ExecuteCommandAsync();

        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException("岗位至少选择一个！");
        }

        await SugarClient.Deleteable<UserJobs>().Where(x => x.UserId == user.Id).ExecuteCommandAsync();
        var userJobs = new List<UserJobs>();
        userJobs.AddRange(user.Jobs.Select(x => new UserJobs() { UserId = user.Id, JobId = x.Id }));
        await SugarClient.Insertable(userJobs).ExecuteCommandAsync();

        //清理缓存
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoByName +
                                           user.Username.ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserRolesById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserJobsById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(
            GlobalConstants.CacheKey.UserPermissionById + user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserBuildMenuById +
                                           user.Id.ToString().ToMd5String16());
        return true;
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        //验证角色等级
        await _roleService.VerificationUserRoleLevelAsync(await _roleService.QueryUserRoleLevelAsync(ids));
        if (ids.Contains(ApeContext.LoginUserInfo.UserId))
        {
            throw new BadRequestException("禁止删除自己");
        }

        var users = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        foreach (var user in users)
        {
            await ClearUserCache(user);
        }

        return await LogicDelete<User>(x => ids.Contains(x.Id)) > 0;
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryAsync(UserQueryCriteria userQueryCriteria, Pagination pagination)
    {
        var whereExpression = await GetWhereExpression(userQueryCriteria);
        Expression<Func<User, Department>> navigationExpression =
            user => user.Dept;
        Expression<Func<User, List<UserJobs>>> navigationUserJobs = user => user.UserJobList;
        Expression<Func<User, List<UserRoles>>> navigationUserRoles = user => user.UserRoleList;
        var users = await SugarRepository.QueryPageListAsync(whereExpression, pagination, null, navigationExpression,
            navigationUserJobs, navigationUserRoles);
        foreach (var user in users)
        {
            user.DeptId = 0;
            //岗位
            var jobIds = user.UserJobList.Select(j => j.JobId).ToList();
            user.Jobs = await _jobService.TableWhere(x => jobIds.Contains(x.Id)).ToListAsync();

            //角色
            var roleIds = user.UserRoleList.Select(r => r.RoleId).ToList();
            user.Roles = await _roleService.TableWhere(x => roleIds.Contains(x.Id)).ToListAsync();
        }

        return ApeContext.Mapper.Map<List<UserDto>>(users);
    }


    public async Task<List<ExportBase>> DownloadAsync(UserQueryCriteria userQueryCriteria)
    {
        //var users = await QueryAsync(userQueryCriteria, new Pagination { PageSize = 9999 });

        var whereExpression = await GetWhereExpression(userQueryCriteria);
        Expression<Func<User, Department>> navigationExpression =
            user => user.Dept;
        Expression<Func<User, List<UserJobs>>> navigationUserJobs = user => user.UserJobList;
        Expression<Func<User, List<UserRoles>>> navigationUserRoles = user => user.UserRoleList;
        var users = await Table.Includes(navigationExpression).Includes(navigationUserJobs)
            .Includes(navigationUserRoles).WhereIF(whereExpression != null, whereExpression).ToListAsync();
        foreach (var user in users)
        {
            user.DeptId = 0;
            //岗位
            var jobIds = user.UserJobList.Select(j => j.JobId).ToList();
            user.Jobs = await _jobService.TableWhere(x => jobIds.Contains(x.Id)).ToListAsync();

            //角色
            var roleIds = user.UserRoleList.Select(r => r.RoleId).ToList();
            user.Roles = await _roleService.TableWhere(x => roleIds.Contains(x.Id)).ToListAsync();
        }

        List<ExportBase> userExports = new List<ExportBase>();
        userExports.AddRange(users.Select(x => new UserExport()
        {
            Id = x.Id,
            Username = x.Username,
            Role = string.Join(",", x.Roles.Select(r => r.Name).ToArray()),
            NickName = x.NickName,
            Phone = x.Phone,
            Email = x.Email,
            Enabled = x.Enabled ? EnabledState.Enabled : EnabledState.Disabled,
            Dept = x.Dept.Name,
            Job = string.Join(",", x.Jobs.Select(j => j.Name).ToArray()),
            Gender = x.Gender,
            CreateTime = x.CreateTime
        }));
        return userExports;
    }

    #endregion

    #region 扩展方法

    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CacheKey.UserInfoById)]
    public async Task<UserDto> QueryByIdAsync(long userId)
    {
        UserDto userDto = null;
        var user = await TableWhere(x => x.Id == userId).FirstAsync();


        if (user != null)
        {
            userDto = ApeContext.Mapper.Map<UserDto>(user);
            await AddUserAttributes(userDto);
        }

        return userDto;
    }

    /// <summary>
    /// 查询用户
    /// </summary>
    /// <param name="userName">邮箱 or 用户名</param>
    /// <returns></returns>
    [UseCache(Expiration = 30, KeyPrefix = GlobalConstants.CacheKey.UserInfoByName)]
    public async Task<UserDto> QueryByNameAsync(string userName)
    {
        User user;
        if (userName.IsEmail())
        {
            user = await SugarRepository.QueryFirstAsync(s => s.Email == userName);
        }
        else
        {
            user = await SugarRepository.QueryFirstAsync(s => s.Username == userName);
        }

        return ApeContext.Mapper.Map<UserDto>(user);
    }


    public async Task<List<UserDto>> QueryByRoleIdAsync(long roleId)
    {
        var users =
            await SugarRepository.QueryMuchAsync<User, UserRoles, User>(
                (u, ur) => new object[]
                {
                    JoinType.Left, u.Id == ur.UserId,
                },
                (u, ur) => u,
                (u, ur) => ur.RoleId == roleId
            );
        return ApeContext.Mapper.Map<List<UserDto>>(users);
    }

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    /// <param name="deptIds"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        return ApeContext.Mapper.Map<List<UserDto>>(
            await SugarRepository.QueryListAsync(u => deptIds.Contains(u.DeptId)));
    }

    /// <summary>
    /// 更新用户公共信息
    /// </summary>
    /// <param name="updateUserCenterDto"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<bool> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto)
    {
        if (updateUserCenterDto.Id != ApeContext.LoginUserInfo.UserId)
            throw new BadRequestException("禁止操作他人数据");

        var user = await TableWhere(x => x.Id == updateUserCenterDto.Id).FirstAsync();
        if (user.IsNull())
            throw new BadRequestException("数据不存在！");
        if (!updateUserCenterDto.Phone.IsPhone())
            throw new BadRequestException("电话格式错误");

        var checkUser = await SugarRepository.QueryFirstAsync(x =>
            x.Phone == updateUserCenterDto.Phone && x.Id != updateUserCenterDto.Id);
        if (checkUser.IsNotNull())
            throw new BadRequestException($"电话=>{checkUser.Phone}=>已存在!");

        user.NickName = updateUserCenterDto.NickName;
        user.Gender = updateUserCenterDto.Gender;
        user.Phone = updateUserCenterDto.Phone;
        return await UpdateEntityAsync(user);
    }

    public async Task<bool> UpdatePasswordAsync(UpdateUserPassDto userPassDto)
    {
        var rsaHelper = new RsaHelper(ApeContext.Configs.Rsa);
        string oldPassword = rsaHelper.Decrypt(userPassDto.OldPassword);
        string newPassword = rsaHelper.Decrypt(userPassDto.NewPassword);
        string confirmPassword = rsaHelper.Decrypt(userPassDto.ConfirmPassword);

        if (oldPassword == newPassword)
            throw new BadRequestException("新密码不能与旧密码相同");

        if (!newPassword.Equals(confirmPassword))
        {
            throw new BadRequestException("两次输入不匹配");
        }

        var curUser = await TableWhere(x => x.Id == ApeContext.LoginUserInfo.UserId).FirstAsync();
        if (curUser.IsNull())
            throw new BadRequestException("数据不存在！");
        if (!BCryptHelper.Verify(oldPassword, curUser.Password))
        {
            throw new BadRequestException("旧密码错误");
        }

        //设置用户密码
        curUser.Password = BCryptHelper.Hash(newPassword);
        curUser.PasswordReSetTime = DateTime.Now;
        var isTrue = await UpdateEntityAsync(curUser);
        if (isTrue)
        {
            //清理缓存
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoById +
                                               curUser.Id.ToString().ToMd5String16());
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoByName +
                                               curUser.Username.ToMd5String16());

            //退出当前用户
            await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.OnlineKey +
                                               ApeContext.HttpUser.JwtToken.ToMd5String16());
        }

        return true;
    }

    /// <summary>
    /// 修改邮箱
    /// </summary>
    /// <param name="updateUserEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateEmailAsync(UpdateUserEmailDto updateUserEmailDto)
    {
        var curUser = await TableWhere(x => x.Id == ApeContext.LoginUserInfo.UserId).FirstAsync();
        if (curUser.IsNull())
            throw new BadRequestException("数据不存在！");
        var rsaHelper = new RsaHelper(ApeContext.Configs.Rsa);
        string password = rsaHelper.Decrypt(updateUserEmailDto.Password);
        if (!BCryptHelper.Verify(password, curUser.Password))
        {
            throw new BadRequestException("密码错误");
        }

        var code = await ApeContext.Cache.GetAsync<string>(
            GlobalConstants.CacheKey.EmailCaptchaKey + updateUserEmailDto.Email.ToMd5String16());
        if (code.IsNullOrEmpty() || !code.Equals(updateUserEmailDto.Code))
        {
            throw new BadRequestException("验证码错误");
        }

        curUser.Email = updateUserEmailDto.Email;
        return await UpdateEntityAsync(curUser);
    }

    public async Task<bool> UpdateAvatarAsync(IFormFile file)
    {
        var curUser = await TableWhere(x => x.Id == ApeContext.LoginUserInfo.UserId).FirstAsync();
        if (curUser.IsNull())
            throw new BadRequestException("数据不存在！");

        string avatarName = GuidHelper.GenerateKey().ToLower() + "_" + file.FileName;
        string avatarPath = Path.Combine(AppSettings.WebRootPath, "file", "avatar");

        if (!Directory.Exists(avatarPath))
        {
            Directory.CreateDirectory(avatarPath);
        }

        avatarPath = Path.Combine(avatarPath, avatarName);
        await using (var fs = new FileStream(avatarPath, FileMode.CreateNew))
        {
            await file.CopyToAsync(fs);
            fs.Flush();
        }

        curUser.AvatarPath = "/file/avatar/";
        curUser.AvatarName = avatarName;
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoById +
                                           curUser.Id.ToString().ToMd5String16());
        return await UpdateEntityAsync(curUser);
    }

    #endregion

    #region 私有方法  补充用户属性

    private async Task AddUserAttributes(UserDto userDto)
    {
        //补充部门岗位属性
        var dept = await _departmentService.QueryByIdAsync(userDto.DeptId);
        userDto.Dept = dept;
        userDto.Jobs = await GetJobListAsync(userDto.Id);
        //用户角色
        userDto.Roles.AddRange(await _roleService.QueryByUserIdAsync(userDto.Id));
    }

    private async Task<List<JobSmallDto>> GetJobListAsync(long userId)
    {
        var jobs = await SugarRepository
            .QueryMuchAsync<User, UserJobs, Job, Job>(
                (u, uj, j) => new object[]
                {
                    JoinType.Left, u.Id == uj.UserId,
                    JoinType.Left, uj.JobId == j.Id
                },
                (u, uj, j) => j,
                (u, uj, j) => u.Id == userId
            );
        return ApeContext.Mapper.Map<List<JobSmallDto>>(jobs);
    }


    private async Task ClearUserCache(User user)
    {
        //清理缓存
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserInfoByName +
                                           user.Username.ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserRolesById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserJobsById +
                                           user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(
            GlobalConstants.CacheKey.UserPermissionById + user.Id.ToString().ToMd5String16());
        await ApeContext.Cache.RemoveAsync(GlobalConstants.CacheKey.UserBuildMenuById +
                                           user.Id.ToString().ToMd5String16());
    }

    #endregion

    #region 条件表达式

    private async Task<Expression<Func<User, bool>>> GetWhereExpression(UserQueryCriteria userQueryCriteria)
    {
        Expression<Func<User, bool>> whereExpression = u => true;
        if (userQueryCriteria.Id > 0)
        {
            whereExpression = whereExpression.AndAlso(u => u.Id == userQueryCriteria.Id);
        }

        if (!userQueryCriteria.Enabled.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(u => u.Enabled == userQueryCriteria.Enabled);
        }

        if (userQueryCriteria.DeptId > 0)
        {
            var depts = await _departmentService.QueryByPIdAsync(userQueryCriteria.DeptId);
            userQueryCriteria.DeptIds = new List<long> { userQueryCriteria.DeptId };
            userQueryCriteria.DeptIds.AddRange(depts.Select(d => d.Id));
            whereExpression = whereExpression.AndAlso(u => userQueryCriteria.DeptIds.Contains(u.DeptId));
        }

        if (!userQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(u =>
                u.Username.Contains(userQueryCriteria.KeyWords) ||
                u.NickName.Contains(userQueryCriteria.KeyWords) || u.Email.Contains(userQueryCriteria.KeyWords));
        }

        if (!userQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(u =>
                u.CreateTime >= userQueryCriteria.CreateTime[0] && u.CreateTime <= userQueryCriteria.CreateTime[1]);
        }

        //数据权限 
        if (!ApeContext.LoginUserInfo.IsNotNull())
        {
            List<long> deptIds =
                await _dataScopeService.GetDeptIds(await QueryByIdAsync(ApeContext.LoginUserInfo.UserId));
            if (deptIds.Count > 0)
            {
                whereExpression = whereExpression.AndAlso(u => deptIds.Contains(u.DeptId));
            }
        }

        return whereExpression;
    }

    #endregion
}
