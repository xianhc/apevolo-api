using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 岗位Dto
/// </summary>
[AutoMapping(typeof(Job), typeof(CreateUpdateJobDto))]
public class CreateUpdateJobDto : BaseEntityDto<long>
{
    //可以重写ErrorMessage消息,"名称"可以为占位符{0}，但是要要设置 [Display(Name = "名称")]
    //[Required(ErrorMessage = "名称为必填项")]
    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Range(1, 999)]
    public int Sort { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }
}
