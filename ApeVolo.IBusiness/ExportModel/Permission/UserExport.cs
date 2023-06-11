using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class UserExport : ExportBase
{
    [Display(Name = "User.Name")]
    public string Username { get; set; }

    [Display(Name = "Role.Name")]
    public string Role { get; set; }

    [Display(Name = "User.NickName")]
    public string NickName { get; set; }

    [Display(Name = "User.Phone")]
    public string Phone { get; set; }

    [Display(Name = "User.Email")]
    public string Email { get; set; }

    [Display(Name = "User.Enabled")]
    public EnabledState Enabled { get; set; }

    [Display(Name = "Dept.Name")]
    public string Dept { get; set; }

    [Display(Name = "Job.Name")]
    public string Job { get; set; }

    [Display(Name = "User.Gender")]
    public string Gender { get; set; }
}