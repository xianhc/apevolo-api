using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 全局设置Dto
/// </summary>
[AutoMapping(typeof(Setting), typeof(CreateUpdateSettingDto))]
public class CreateUpdateSettingDto : BaseEntityDto<long>
{
    /// <summary>
    /// 键
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [Required]
    public string Value { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }
}
