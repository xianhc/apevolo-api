using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Common.Enums;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 字典Dto
/// </summary>
[AutoMapping(typeof(Dict), typeof(CreateUpdateDictDto))]
public class CreateUpdateDictDto : BaseEntityDto<long>
{
    /// <summary>
    /// 名称
    /// </summary>
    public DictType DictType { get; set; } = DictType.System;

    /// <summary>
    /// 名称
    /// </summary>
    [Required]

    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// 字典详情
    /// </summary>
    public List<CreateUpdateDictDetailDto> DictDetails { get; set; }
}
