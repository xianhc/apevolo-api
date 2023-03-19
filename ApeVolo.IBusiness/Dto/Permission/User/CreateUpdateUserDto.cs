using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.User;

[AutoMapping(typeof(Entity.Permission.User.User), typeof(CreateUpdateUserDto))]
public class CreateUpdateUserDto : EntityDtoRoot<long>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Display(Name = "User.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Display(Name = "User.NickName")]
    [Required(ErrorMessage = "{0}required")]
    public string NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Display(Name = "User.Email")]
    [Required(ErrorMessage = "{0}required")]
    [EmailAddress(ErrorMessage = "{0}ValueIsInvalidAccessor")]
    public string Email { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    [Display(Name = "User.Phone")]
    [Required(ErrorMessage = "{0}required")]
    [RegularExpression(@"^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\d{8}$",
        ErrorMessage = "{0}ValueIsInvalidAccessor")]
    public string Phone { get; set; }

    [Display(Name = "User.Gender")]
    [Required(ErrorMessage = "{0}required")]
    public string Gender { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    [Required(ErrorMessage = "{0}required")]
    public UserDept Dept { get; set; }

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