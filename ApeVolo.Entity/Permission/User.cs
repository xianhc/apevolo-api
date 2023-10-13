using System;
using System.Collections.Generic;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 系统用户
/// </summary>
[SugarTable("sys_user")]
public class User : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string Email { get; set; }

    /// <summary>
    /// 是否内置管理员
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool Enabled { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public string Password { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long DeptId { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 11, IsNullable = true)]
    public string Phone { get; set; }

    /// <summary>
    /// 头像路径
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string AvatarPath { get; set; }

    /// <summary>
    /// 最后修改密码时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? PasswordReSetTime { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public bool Sex { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool IsDeleted { get; set; }


    #region 扩展属性

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(DeptId))]
    public Department Dept { get; set; }


    /// <summary>
    /// 用户角色集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToMany, nameof(UserRoles.UserId))]
    public List<UserRoles> UserRoleList { get; set; }

    /// <summary>
    /// 角色集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Role> Roles { get; set; }

    /// <summary>
    /// 用户岗位集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToMany, nameof(UserJobs.UserId))]
    public List<UserJobs> UserJobList { get; set; }

    /// <summary>
    /// 岗位集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Job> Jobs { get; set; }

    #endregion
}
