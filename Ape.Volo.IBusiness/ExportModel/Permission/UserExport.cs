using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Permission;

/// <summary>
/// 用户导出模板
/// </summary>
public class UserExport : ExportBase
{
    /// <summary>
    /// 用户名称
    /// </summary>
    [Display(Name = "用户名称")]
    public string Username { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Display(Name = "角色名称")]
    public string Role { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    [Display(Name = "用户昵称")]
    public string NickName { get; set; }

    /// <summary>
    /// 用户电话
    /// </summary>
    [Display(Name = "用户电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 用户邮箱
    /// </summary>
    [Display(Name = "用户邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Display(Name = "是否激活")]
    public EnabledState Enabled { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [Display(Name = "部门名称")]
    public string Dept { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    [Display(Name = "岗位名称")]
    public string Job { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Display(Name = "性别")]
    public string Gender { get; set; }
}
