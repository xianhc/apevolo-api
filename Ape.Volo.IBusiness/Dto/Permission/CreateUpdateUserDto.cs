using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(User), typeof(CreateUpdateUserDto))]
public class CreateUpdateUserDto : BaseEntityDto<long>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Required]
    public string NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [Required]
    [RegularExpression(@"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$")]
    public string Phone { get; set; }

    [Display(Name = "User.Gender")]
    [Required(ErrorMessage = "{0}required")]
    public string Gender { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public UserDeptDto Dept { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public List<UserRoleDto> Roles { get; set; }

    /// <summary>
    /// 岗位
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public List<UserJobDto> Jobs { get; set; }
}
