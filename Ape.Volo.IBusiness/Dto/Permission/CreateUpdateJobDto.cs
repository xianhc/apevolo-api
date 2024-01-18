using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

[AutoMapping(typeof(ApeVolo.Entity.Permission.Job), typeof(CreateUpdateJobDto))]
public class CreateUpdateJobDto : BaseEntityDto<long>
{
    //可以重写ErrorMessage消息,"名称"可以为占位符{0}，但是要要设置 [Display(Name = "名称")]
    //[Required(ErrorMessage = "名称为必填项")]
    [Required]
    public string Name { get; set; }

    [Range(1, 999)]
    public int Sort { get; set; }

    public bool Enabled { get; set; }
}
