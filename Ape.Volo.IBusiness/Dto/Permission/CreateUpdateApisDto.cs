using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.Permission;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.Permission;

/// <summary>
/// 
/// </summary>
[AutoMapping(typeof(Apis), typeof(CreateUpdateApisDto))]
public class CreateUpdateApisDto : BaseEntityDto<long>
{
    /// <summary>
    /// 组
    /// </summary>
    [Required]
    public string Group { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Required]
    public string Url { get; set; }


    /// <summary>
    /// 描述
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [Required]
    public string Method { get; set; }
}
