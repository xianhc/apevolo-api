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
using ApeVolo.Common.Helper.Excel;
using ApeVolo.Common.Model;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Do.Core;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.EditDto.Core;
using ApeVolo.IBusiness.Interface.Core;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Core;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace ApeVolo.Business.Impl.Core;

/// <summary>
/// 用户服务
/// </summary>
public class UserService : BaseServices<User>, IUserService
{
    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;
    private readonly IUserRolesService _userRoleService;
    private readonly IUserJobsService _userJobsService;
    private readonly IJobService _jobService;
    private readonly IDataScopeService _dataScopeService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public UserService(IMapper mapper, IUserRepository userRepository, IDepartmentService departmentService,
        IRoleService roleService, IJobService jobService,
        IUserRolesService userRoleService, IUserJobsService userJobsService, IDataScopeService dataScopeService,
        ICurrentUser currentUser, IWebHostEnvironment webHostEnvironment, IRedisCacheService redisCacheService)
    {
        _mapper = mapper;
        _baseDal = userRepository;
        _currentUser = currentUser;
        _departmentService = departmentService;
        _roleService = roleService;
        _jobService = jobService;
        _userRoleService = userRoleService;
        _userJobsService = userJobsService;
        _dataScopeService = dataScopeService;
        _webHostEnvironment = webHostEnvironment;
        _redisCacheService = redisCacheService;
    }

    #endregion

    #region 基础方法

    [UseTran]
    public async Task<bool> CreateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        if (await IsExistAsync(x => x.IsDeleted == false
                                    && x.Username == createUpdateUserDto.Username))
        {
            throw new BadRequestException($"用户名称=>{createUpdateUserDto.Username}=>已存在！");
        }

        if (await IsExistAsync(x => x.IsDeleted == false
                                    && x.Email == createUpdateUserDto.Email))
        {
            throw new BadRequestException($"邮箱=>{createUpdateUserDto.Email}=>已存在！");
        }

        if (await IsExistAsync(x => x.IsDeleted == false
                                    && x.Phone == createUpdateUserDto.Phone))
        {
            throw new BadRequestException($"电话=>{createUpdateUserDto.Phone}=>已存在！");
        }

        var user = _mapper.Map<User>(createUpdateUserDto);

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
            throw new BadRequestException("角色至少选择一个！");
        }

        var userRoles = new List<CreateUpdateUserRolesDto>();
        userRoles.AddRange(user.Roles.Select(x => new CreateUpdateUserRolesDto(user.Id, x.Id)));
        await _userRoleService.CreateAsync(userRoles);


        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException("岗位至少选择一个！");
        }

        var userJobs = new List<CreateUpdateUserJobsDto>();
        userJobs.AddRange(createUpdateUserDto.Jobs.Select(x => new CreateUpdateUserJobsDto(user.Id, x.Id)));
        await _userJobsService.CreateAsync(userJobs);

        return true;
    }

    [UseTran]
    public async Task<bool> UpdateAsync(CreateUpdateUserDto createUpdateUserDto)
    {
        //取出待更新数据
        var oldUser = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == createUpdateUserDto.Id);
        if (oldUser.IsNull())
        {
            throw new BadRequestException("更新失败=》待更新数据不存在！");
        }

        if (oldUser.Username != createUpdateUserDto.Username && await IsExistAsync(x => x.IsDeleted == false
                && x.Username == createUpdateUserDto.Username))
        {
            throw new BadRequestException($"用户名称=>{createUpdateUserDto.Username}=>已存在！");
        }

        if (oldUser.Email != createUpdateUserDto.Email && await IsExistAsync(x => x.IsDeleted == false
                && x.Email == createUpdateUserDto.Email))
        {
            throw new BadRequestException($"邮箱=>{createUpdateUserDto.Email}=>已存在！");
        }

        if (oldUser.Phone != createUpdateUserDto.Phone && await IsExistAsync(x => x.IsDeleted == false
                && x.Phone == createUpdateUserDto.Phone))
        {
            throw new BadRequestException($"电话=>{createUpdateUserDto.Phone}=>已存在！");
        }

        //验证角色等级
        var levels = (await _roleService.QueryByUserIdAsync(oldUser.Id)).Select(x => x.Level);
        await _roleService.VerificationUserRoleLevelAsync(levels.Min());
        var user = _mapper.Map<User>(createUpdateUserDto);
        user.DeptId = user.Dept.Id;
        //更新用户
        await UpdateEntityAsync(user, new List<string> { "password", "salt_key", "avatar_name", "avatar_path" });
        //角色
        if (user.Roles.Count < 1)
        {
            throw new BadRequestException("角色至少选择一个！");
        }

        await _userRoleService.DeleteByUserIdAsync(user.Id);
        var userRoles = new List<CreateUpdateUserRolesDto>();
        userRoles.AddRange(user.Roles.Select(x => new CreateUpdateUserRolesDto(user.Id, x.Id)));
        await _userRoleService.CreateAsync(userRoles);

        //岗位
        if (user.Jobs.Count < 1)
        {
            throw new BadRequestException("岗位至少选择一个！");
        }

        await _userJobsService.DeleteByUserIdAsync(user.Id);
        var userJobs = new List<CreateUpdateUserJobsDto>();
        userJobs.AddRange(user.Jobs.Select(x => new CreateUpdateUserJobsDto(user.Id, x.Id)));
        await _userJobsService.CreateAsync(userJobs);

        //清理缓存
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + user.Username.ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserRolesById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserJobsById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserPermissionById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserBuildMenuById + user.Id.ToString().ToMd5String());
        return true;
    }

    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        //验证角色等级
        await _roleService.VerificationUserRoleLevelAsync(await _roleService.QueryUserRoleLevelAsync(ids));
        var users = await QueryByIdsAsync(ids);
        users.ForEach(ClearUserCache);
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
        Expression<Func<User, bool>> whereExpression = u => u.IsDeleted == false;
        if (userQueryCriteria.Id > 0)
        {
            whereExpression = whereExpression.And(u => u.Id == userQueryCriteria.Id);
        }

        if (!userQueryCriteria.Enabled.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(u => u.Enabled == userQueryCriteria.Enabled);
        }

        if (userQueryCriteria.DeptId > 0)
        {
            var depts = await _departmentService.QueryByPIdAsync(userQueryCriteria.DeptId);
            userQueryCriteria.DeptIds = new List<long> { userQueryCriteria.DeptId };
            userQueryCriteria.DeptIds.AddRange(depts.Select(d => d.Id));
            whereExpression = whereExpression.And(u => userQueryCriteria.DeptIds.Contains(u.DeptId));
        }

        if (!userQueryCriteria.KeyWords.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(u =>
                u.Username.Contains(userQueryCriteria.KeyWords) ||
                u.NickName.Contains(userQueryCriteria.KeyWords) || u.Email.Contains(userQueryCriteria.KeyWords));
        }

        if (!userQueryCriteria.CreateTime.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(u =>
                u.CreateTime >= userQueryCriteria.CreateTime[0] && u.CreateTime <= userQueryCriteria.CreateTime[1]);
        }

        //数据权限 
        if (!_currentUser.Id.IsNullOrEmpty())
        {
            List<long> deptIds = await _dataScopeService.GetDeptIds(await QueryByIdAsync(_currentUser.Id));
            if (deptIds.Count > 0)
            {
                whereExpression = whereExpression.And(u => deptIds.Contains(u.DeptId));
            }
        }

        var users = await _baseDal.QueryMapperPageListAsync(async (it, cache) =>
        {
            //部门 
            var department = cache.GetListByPrimaryKeys<Department>(model => model.DeptId);

            //岗位
            var userJobs = await _userJobsService.QueryByUserIdAsync(it.Id);
            var jobIds = userJobs.Select(j => j.JobId).ToList();
            var jobs = await _jobService.QueryByIdsAsync(jobIds);

            //角色
            var userRoles = await _userRoleService.QueryAsync(it.Id);
            var roleIds = userRoles.Select(r => r.RoleId).ToList();
            var roles = await _roleService.QueryByIdsAsync(roleIds);

            it.Dept = department.FirstOrDefault(d => d.Id == it.DeptId);
            it.Jobs = jobs;
            it.Roles = roles;
            it.DeptId = 0;
        }, whereExpression, pagination);

        return _mapper.Map<List<UserDto>>(users);
    }


    public async Task<List<ExportRowModel>> DownloadAsync(UserQueryCriteria userQueryCriteria)
    {
        var users = await QueryAsync(userQueryCriteria, new Pagination { PageSize = 9999 });
        List<ExportRowModel> exportRowModels = new List<ExportRowModel>();
        foreach (var item in users)
        {
            var point = 0;
            var exportColumnModels = new List<ExportColumnModel>
            {
                new() { Key = "ID", Value = item.Id.ToString(), Point = point++ },
                new() { Key = "用户名", Value = item.Username, Point = point++ },
                new()
                {
                    Key = "角色",
                    Value = string.Join(",", item.Roles.Select(x => x.Name).ToArray()),
                    Point = point++
                },
                new() { Key = "昵称", Value = item.NickName, Point = point++ },
                new() { Key = "电话", Value = item.Phone, Point = point++ },
                new() { Key = "邮箱", Value = item.Email, Point = point++ },
                new() { Key = "状态", Value = item.Enabled ? "激活" : "停用", Point = point++ },
                new() { Key = "部门", Value = item.Dept.Name, Point = point++ },
                new()
                {
                    Key = "岗位",
                    Value = string.Join(",", item.Jobs.Select(x => x.Name).ToArray()),
                    Point = point++
                },
                new() { Key = "性别", Value = item.Gender, Point = point++ },
                new()
                {
                    Key = "创建时间", Value = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), Point = point
                }
            };
            exportRowModels.Add(new ExportRowModel { exportColumnModels = exportColumnModels });
        }

        return exportRowModels;
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
            userDto = _mapper.Map<UserDto>(user);
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
        User user;
        if (userName.IsEmail())
        {
            user = await _baseDal.QueryFirstAsync(s => s.Email == userName && s.IsDeleted == false);
        }
        else
        {
            user = await _baseDal.QueryFirstAsync(s => s.Username == userName && s.IsDeleted == false);
        }

        return _mapper.Map<UserDto>(user);
    }


    public async Task<List<UserDto>> QueryByRoleIdAsync(long roleId)
    {
        var users = await _baseDal.QueryMuchAsync<User, UserRoles, User>(
            (u, ur) => new object[]
            {
                JoinType.Left, u.Id == ur.UserId,
            },
            (u, ur) => u,
            (u, ur) => u.IsDeleted == false && ur.RoleId == roleId
        );
        return _mapper.Map<List<UserDto>>(users);
    }

    /// <summary>
    /// 根据部门ID查找用户
    /// </summary>
    /// <param name="deptIds"></param>
    /// <returns></returns>
    public async Task<List<UserDto>> QueryByDeptIdsAsync(List<long> deptIds)
    {
        return _mapper.Map<List<UserDto>>(
            await _baseDal.QueryListAsync(u => u.IsDeleted == false && deptIds.Contains(u.DeptId)));
    }

    /// <summary>
    /// 更新用户公共信息
    /// </summary>
    /// <param name="updateUserCenterDto"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<bool> UpdateCenterAsync(UpdateUserCenterDto updateUserCenterDto)
    {
        if (updateUserCenterDto.Id != _currentUser.Id)
            throw new BadRequestException("You do not have the right to modify other user data");

        var user = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == updateUserCenterDto.Id);
        if (user.IsNull())
            throw new BadRequestException(nameof(user) + " does not exist");
        if (!updateUserCenterDto.Phone.IsPhone())
            throw new BadRequestException(nameof(updateUserCenterDto.Phone) +
                                          " please enter the correct phone number！");

        var checkUser = await _baseDal.QueryFirstAsync(x => x.Phone == updateUserCenterDto.Phone);
        if (checkUser.IsNotNull())
            throw new BadRequestException($"Mobile phone number {updateUserCenterDto.Phone} already exists");

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

        if (oldPassword == newPassword)
            throw new BadRequestException("新密码不能与旧密码相同！");

        var curUser = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == _currentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException(nameof(curUser) + " does not exist");
        if (curUser.Password !=
            (oldPassword + curUser.SaltKey).ToHmacsha256String(
                AppSettings.GetValue(new[] { "HmacSecret" })))
        {
            throw new BadRequestException("修改失败，旧密码错误！");
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
            await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + curUser.Id.ToString().ToMd5String());
            await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + curUser.Username.ToMd5String());

            //退出当前用户
            await _redisCacheService.RemoveAsync(RedisKey.OnlineKey + _currentUser.GetToken());
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
        var curUser = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == _currentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException("用户不存在！");
        var jsEncryptHelper = new JsEncryptHelper();
        string password = jsEncryptHelper.Decrypt(updateUserEmailDto.Password);
        if (curUser.Password !=
            (password + curUser.SaltKey).ToHmacsha256String(AppSettings.GetValue("HmacSecret")))
        {
            throw new BadRequestException("修改失败，密码错误！");
        }

        var code = await _redisCacheService.GetCacheAsync(
            RedisKey.EmailCaptchaKey + updateUserEmailDto.Email.ToMd5String());
        if (code.IsNullOrEmpty() || !code.Equals(updateUserEmailDto.Code))
        {
            throw new BadRequestException("修改失败，验证码错误或已失效！");
        }

        curUser.Email = updateUserEmailDto.Email;
        return await UpdateEntityAsync(curUser);
    }

    public async Task<bool> UpdateAvatarAsync(IFormFile file)
    {
        var curUser = await QueryFirstAsync(x => x.IsDeleted == false && x.Id == _currentUser.Id);
        if (curUser.IsNull())
            throw new BadRequestException("用户不存在！");

        string avatarName = GuidHelper.GenerateKey() + "_" + file.FileName;
        string avatarPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "images", "avatar");

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

        //curUser.AvatarPath = avatarPath;
        curUser.AvatarPath = "/images/avatar/";
        curUser.AvatarName = avatarName;
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + curUser.Id.ToString().ToMd5String());
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
        var jobs = await _baseDal.QueryMuchAsync<User, UserJobs, Job, Job>(
            (u, uj, j) => new object[]
            {
                JoinType.Left, u.Id == uj.UserId,
                JoinType.Left, uj.JobId == j.Id
            },
            (u, uj, j) => j,
            (u, uj, j) => u.IsDeleted == false && j.IsDeleted == false && u.Id == userId
        );
        return _mapper.Map<List<JobSmallDto>>(jobs);
    }


    private async void ClearUserCache(User user)
    {
        //清理缓存
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserInfoByName + user.Username.ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserRolesById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserJobsById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserPermissionById + user.Id.ToString().ToMd5String());
        await _redisCacheService.RemoveAsync(RedisKey.UserBuildMenuById + user.Id.ToString().ToMd5String());
    }

    #endregion
}