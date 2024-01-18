using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.Permission;

public class UserExport : ExportBase
{
    [Display(Name = "用户名称")]
    public string Username { get; set; }

    [Display(Name = "角色名称")]
    public string Role { get; set; }

    [Display(Name = "用户昵称")]
    public string NickName { get; set; }

    [Display(Name = "用户电话")]
    public string Phone { get; set; }

    [Display(Name = "用户邮箱")]
    public string Email { get; set; }

    [Display(Name = "是否激活")]
    public EnabledState Enabled { get; set; }

    [Display(Name = "部门名称")]
    public string Dept { get; set; }

    [Display(Name = "岗位名称")]
    public string Job { get; set; }

    [Display(Name = "性别")]
    public string Gender { get; set; }
}
