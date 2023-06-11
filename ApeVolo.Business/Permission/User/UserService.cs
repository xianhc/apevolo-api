using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Permission.User;
using ApeVolo.IBusiness.Dto.Permission.Job;
using ApeVolo.IBusiness.Dto.Permission.User;
using ApeVolo.IBusiness.ExportModel.Permission;
using ApeVolo.IBusiness.Interface.Permission.Department;
using ApeVolo.IBusiness.Interface.Permission.Job;
using ApeVolo.IBusiness.Interface.Permission.Role;
using ApeVolo.IBusiness.Interface.Permission.User;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Permission.User;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace ApeVolo.Business.Permission.User;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseServices<Entity.Permission.User.User>, IUserService
{
    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;
    private readonly IUserRolesService _userRoleService;
    private readonly IUserJobsService _userJobsService;
    private readonly IJobService _jobService;
    private readonly IDataScopeService _dataScopeService;
    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public UserService(IMapper mapper, IUserRepository userRepository, IDepartmentService departmentService,
        IRoleService roleService, IJobService jobService,
        IUserRolesService userRoleService, IUserJobsService userJobsService, IDataScopeService dataScopeService,
        ICurrentUser currentUser, IRedisCacheService redisCacheService)
    {
        Mapper = mapper;
        BaseDal = userRepository;
        CurrentUser = currentUser;
        _departmentService = departmentService;
        _roleService = roleService;
        _jobService = jobService;
        _userRoleService = userRoleService;
        _userJobsService = userJobsService;
        _dataScopeService = dataScopeService;
        _redisCacheService = redisCacheService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        if (await IsExistAsync(x => x.Username == createUpdateUserDto.Username))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Username));
        }

        if (await IsExistAsync(x => x.Email == createUpdateUserDto.Email))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Email));
        }

        if (await IsExistAsync(x => x.Phone == createUpdateUserDto.Phone))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Phone));
        }

        var user = Mapper.Map<Entity.Permission.User.User>(createUpdateUserDto);

        //设置用户密码
        user.SaltKey = SaltKeyHelper.CreateSalt(6);
        user.Password =
            ("123456" + user.SaltKey).ToHmacsha256String(AppSettings.GetValue(new[] { "HmacSecret" }));
        user.DeptId = user.Dept.Id;
        //用户
        await AddEntityAsync(user);

        //角色
        if (user.Roles.Count < 1)
        {
            throw new BadRequestException(Localized.Get("AtLeastOne", Localized.Get("Role")));
        }

        var userRoles = new List<CreateUpdateUserRolesDto>();
        userRoles.AddRange(user.Roles.Select(x => new CreateUpdateUserRolesDto(user.Id, x.Id)));
        await _userRoleService.CreateAsync(userRoles);


        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException(Localized.Get("AtLeastOne", Localized.Get("Job")));
        }

        var userJobs = new List<CreateUpdateUserJobsDto>();
        userJobs.AddRange(user.Jobs.Select(x => new CreateUpdateUserJobsDto(user.Id, x.Id)));
        await _userJobsService.CreateAsync(userJobs);

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        //取出待更新数据
        var oldUser = await QueryFirstAsync(x => x.Id == createUpdateUserDto.Id);
        if (oldUser.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (oldUser.Username != createUpdateUserDto.Username &&
            await IsExistAsync(x => x.Username == createUpdateUserDto.Username))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Username));
        }

        if (oldUser.Email != createUpdateUserDto.Email && await IsExistAsync(x => x.Email == createUpdateUserDto.Email))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Email));
        }

        if (oldUser.Phone != createUpdateUserDto.Phone && await IsExistAsync(x => x.Phone == createUpdateUserDto.Phone))
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                createUpdateUserDto.Phone));
        }

        //验证角色等级
        var levels = (await _roleService.QueryByUserIdAsync(oldUser.Id)).Select(x => x.Level);
        await _roleService.VerificationUserRoleLevelAsync(levels.Min());
        var user = Mapper.Map<Entity.Permission.User.User>(createUpdateUserDto);
        user.DeptId = user.Dept.Id;
        //更新用户
        await UpdateEntityAsync(user, new List<string> { "password", "salt_key", "avatar_name", "avatar_path" });
        //角色
        if (user.Roles.Count < 1)
        {
            throw new BadRequestException(Localized.Get("AtLeastOne", Localized.Get("Role")));
        }

        await _userRoleService.DeleteByUserIdAsync(user.Id);
        var userRoles = new List<CreateUpdateUserRolesDto>();
        userRoles.AddRange(user.Roles.Select(x => new CreateUpdateUserRolesDto(user.Id, x.Id)));
        await _userRoleService.CreateAsync(userRoles);

        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException(Localized.Get("AtLeastOne", Localized.Get("Job")));
        }

        await _userJobsService.DeleteByUserIdAsync(user.Id);
        var userJobs = new List<CreateUpdateUserJobsDto>();
        userJobs.AddRange(user.Jobs.Select(x => new CreateUpdateUserJobsDto(user.Id, x.Id)));
        await _userJobsService.CreateAsync(userJobs);

        //清理缓存
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + user.Username.ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserRolesById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserJobsById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserPermissionById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserBuildMenuById + user.Id.ToString().ToMd5String16());
        return true;
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        //验证角色等级
        await _roleService.VerificationUserRoleLevelAsync(await _roleService.QueryUserRoleLevelAsync(ids));
        if (ids.Contains(CurrentUser.Id))
        {
            throw new BadRequestException("ForbidToDeleteYourself");
        }

        var users = await QueryByIdsAsync(ids);
        foreach (var user in users)
        {
            await ClearUserCache(user);
        }

        return await DeleteEntityListAsync(users);
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="userQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryAsync(UserQueryCriteria userQueryCriteria, Pagination pagination)
    {
        Expression<Func<Entity.Permission.User.User, bool>> whereExpression = u => true;
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
        if (!CurrentUser.Id.IsNullOrEmpty())
        {
            List<long> deptIds = await _dataScopeService.GetDeptIds(await QueryByIdAsync(CurrentUser.Id));
            if (deptIds.Count > 0)
            {
                whereExpression = whereExpression.AndAlso(u => deptIds.Contains(u.DeptId));
            }
        }

        Expression<Func<Entity.Permission.User.User, Entity.Permission.Department>> navigationExpression =
            user => user.Dept;
        Expression<Func<Entity.Permission.User.User, List<UserJobs>>> navigationUserJobs = user => user.UserJobList;
        Expression<Func<Entity.Permission.User.User, List<UserRoles>>> navigationUserRoles = user => user.UserRoleList;
        var users = await BaseDal.QueryPageListAsync(whereExpression, pagination, null, navigationExpression,
            navigationUserJobs, navigationUserRoles);
        foreach (var user in users)
        {
            user.DeptId = 0;
            //岗位
            var jobIds = user.UserJobList.Select(j => j.JobId).ToList();
            user.Jobs = await _jobService.QueryByIdsAsync(jobIds);

            //角色
            var roleIds = user.UserRoleList.Select(r => r.RoleId).ToList();
            user.Roles = await _roleService.QueryByIdsAsync(roleIds);
        }

        return Mapper.Map<List<UserDto>>(users);
    }


    public async Task<List<ExportBase>> DownloadAsync(UserQueryCriteria userQueryCriteria)
    {
        var users = await QueryAsync(userQueryCriteria, new Pagination { PageSize = 9999 });
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

    [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.UserInfoById)]
    public async Task<UserDto> QueryByIdAsync(long userId)
    {
        UserDto userDto = null;
        var user = await QuerySingleAsync(userId);

        if (user != null)
        {
            userDto = Mapper.Map<UserDto>(user);
            await AddUserAttributes(userDto);
        }

        return userDto;
    }

    /// <summary>
    /// 查询用户
    /// </summary>
    /// <param name="userName">邮箱 or 用户名</param>
    /// <returns></returns>
    [RedisCaching(Expiration = 30, KeyPrefix = RedisKey.UserInfoByName)]
    public async Task<UserDto> QueryByNameAsync(string userName)
    {
        Entity.Permission.User.User user;
        if (userName.IsEmail())
        {
            user = await BaseDal.QueryFirstAsync(s => s.Email == userName);
        }
        else
        {
            user = await BaseDal.QueryFirstAsync(s => s.Username == userName);
        }

        return Mapper.Map<UserDto>(user);
    }


    public async Task<List<UserDto>> QueryByRoleIdAsync(long roleId)
    {
        var users = await BaseDal.QueryMuchAsync<Entity.Permission.User.User, UserRoles, Entity.Permission.User.User>(
            (u, ur) => new object[]
            {
                JoinType.Left, u.Id == ur.UserId,
            },
            (u, ur) => u,
            (u, ur) => ur.RoleId == roleId
        );
        return Mapper.Map<List<UserDto>>(users);
    }

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    /// <param name="deptIds"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        return Mapper.Map<List<UserDto>>(
            await BaseDal.QueryListAsync(u => deptIds.Contains(u.DeptId)));
    }

    /// <summary>
    /// 更新用户公共信息
    /// </summary>
    /// <param name="updateUserCenterDto"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<bool> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto)
    {
        if (updateUserCenterDto.Id != CurrentUser.Id)
            throw new BadRequestException(Localized.Get("OperationProhibited"));

        var user = await QueryFirstAsync(x => x.Id == updateUserCenterDto.Id);
        if (user.IsNull())
            throw new BadRequestException(Localized.Get("DataNotExist"));
        if (!updateUserCenterDto.Phone.IsPhone())
            throw new BadRequestException(Localized.Get("ValueIsInvalidAccessor", "Phone"));

        var checkUser = await BaseDal.QueryFirstAsync(x =>
            x.Phone == updateUserCenterDto.Phone && x.Id != updateUserCenterDto.Id);
        if (checkUser.IsNotNull())
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("User"),
                checkUser.Phone));

        user.NickName = updateUserCenterDto.NickName;
        user.Gender = updateUserCenterDto.Gender;
        user.Phone = updateUserCenterDto.Phone;
        return await UpdateEntityAsync(user);
    }

    public async Task<bool> UpdatePasswordAsync(UpdateUserPassDto userPassDto)
    {
        var jsEncryptHelper = new JsEncryptHelper();
        string oldPassword = jsEncryptHelper.Decrypt(userPassDto.OldPassword);
        string newPassword = jsEncryptHelper.Decrypt(userPassDto.NewPassword);
        string confirmPassword = jsEncryptHelper.Decrypt(userPassDto.ConfirmPassword);

        if (oldPassword == newPassword)
            throw new BadRequestException(Localized.Get("PasswordsCannotBeTheSame"));

        if (!newPassword.Equals(confirmPassword))
        {
            throw new BadRequestException(Localized.Get("FailedVerificationTwice"));
        }

        var curUser = await QueryFirstAsync(x => x.Id == CurrentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException(Localized.Get("DataNotExist"));
        if (curUser.Password !=
            (oldPassword + curUser.SaltKey).ToHmacsha256String(
                AppSettings.GetValue(new[] { "HmacSecret" })))
        {
            throw new BadRequestException(Localized.Get("PasswrodWrong"));
        }

        //设置用户密码
        curUser.SaltKey = SaltKeyHelper.CreateSalt(6);
        curUser.Password =
            (newPassword + curUser.SaltKey).ToHmacsha256String(
                AppSettings.GetValue(new[] { "HmacSecret" }));
        curUser.PasswordReSetTime = DateTime.Now;
        var isTrue = await UpdateEntityAsync(curUser);
        if (isTrue)
        {
            //清理缓存
            await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + curUser.Id.ToString().ToMd5String16());
            await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + curUser.Username.ToMd5String16());

            //退出当前用户
            await _redisCacheService.RemoveAsync(RedisKey.OnlineKey + CurrentUser.GetToken().ToMd5String16());
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
        var curUser = await QueryFirstAsync(x => x.Id == CurrentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException(Localized.Get("DataNotExist"));
        var jsEncryptHelper = new JsEncryptHelper();
        string password = jsEncryptHelper.Decrypt(updateUserEmailDto.Password);
        if (curUser.Password !=
            (password + curUser.SaltKey).ToHmacsha256String(AppSettings.GetValue("HmacSecret")))
        {
            throw new BadRequestException(Localized.Get("PasswrodWrong"));
        }

        var code = await _redisCacheService.GetCacheAsync(
            RedisKey.EmailCaptchaKey + updateUserEmailDto.Email.ToMd5String16());
        if (code.IsNullOrEmpty() || !code.Equals(updateUserEmailDto.Code))
        {
            throw new BadRequestException(Localized.Get("CodeWrong"));
        }

        curUser.Email = updateUserEmailDto.Email;
        return await UpdateEntityAsync(curUser);
    }

    public async Task<bool> UpdateAvatarAsync(IFormFile file)
    {
        var curUser = await QueryFirstAsync(x => x.Id == CurrentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException(Localized.Get("DataNotExist"));

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
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + curUser.Id.ToString().ToMd5String16());
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
        var jobs = await BaseDal
            .QueryMuchAsync<Entity.Permission.User.User, UserJobs, Entity.Permission.Job, Entity.Permission.Job>(
                (u, uj, j) => new object[]
                {
                    JoinType.Left, u.Id == uj.UserId,
                    JoinType.Left, uj.JobId == j.Id
                },
                (u, uj, j) => j,
                (u, uj, j) => u.Id == userId
            );
        return Mapper.Map<List<JobSmallDto>>(jobs);
    }


    private async Task ClearUserCache(Entity.Permission.User.User user)
    {
        //清理缓存
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + user.Username.ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserRolesById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserJobsById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserPermissionById + user.Id.ToString().ToMd5String16());
        await _redisCacheService.RemoveAsync(RedisKey.UserBuildMenuById + user.Id.ToString().ToMd5String16());
    }

    #endregion
}