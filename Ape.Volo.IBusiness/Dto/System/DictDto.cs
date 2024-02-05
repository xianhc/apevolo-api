using System.Collections.Generic;
using Ape.Volo.Common.AttributeExt;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 字典Dto
/// </summary>
[AutoMapping(typeof(Dict), typeof(DictDto))]
public class DictDto : BaseEntityDto<long>
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 字典详情
    /// </summary>
    public List<DictDetailDto> DictDetails { get; set; }
}
