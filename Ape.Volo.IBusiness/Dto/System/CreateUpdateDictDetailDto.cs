using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 字典详情Dto
/// </summary>
[AutoMapping(typeof(DictDetail), typeof(CreateUpdateDictDetailDto))]
public class CreateUpdateDictDetailDto : BaseEntityDto<long>
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [Required]
    public long DictId { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Required]
    public string Label { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [Required]
    public string Value { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int DictSort { get; set; }
}
