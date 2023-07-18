using System;
using System.Collections.Generic;
using ApeVolo.Common.DI;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Base;
using SqlSugar;

namespace ApeVolo.Entity.Permission;

/// <summary>
/// 系统用户
/// </summary>
[SugarTable("sys_user", "系统用户")]
public class User : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "用户名")]
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "昵称")]
    public string NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 是否内置管理员
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "内置管理员,无视系统权限")]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "密码")]
    public string Password { get; set; }

    /// <summary>
    /// 签名随机盐
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDescription = "随机盐值")]
    public string SaltKey { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDataType = "bigint", IsNullable = false, ColumnDescription = "部门ID")]
    public long DeptId { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [SugarColumn(Length = 11, IsNullable = true)]
    public string Phone { get; set; }

    /// <summary>
    /// 头像名称
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "头像名称")]
    public string AvatarName { get; set; }

    /// <summary>
    /// 头像路径
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "头像路径")]
    public string AvatarPath { get; set; }

    /// <summary>
    /// 最后修改密码时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "最后修改密码时间")]
    public DateTime? PasswordReSetTime { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "性别")]
    public bool Sex { get; set; }

    [SugarColumn(ColumnDescription = "性别")]
    public string Gender { get; set; }

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
