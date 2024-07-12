using Ape.Volo.Common.Attributes;
using Ape.Volo.Entity.System;
using Ape.Volo.IBusiness.Base;
using Newtonsoft.Json;

namespace Ape.Volo.IBusiness.Dto.System;

/// <summary>
/// 字典详情Dto
/// </summary>
[AutoMapping(typeof(DictDetail), typeof(DictDetailDto))]
public class DictDetailDto : BaseEntityDto<long>
{
    /// <summary>
    /// 字典ID
    /// </summary>
    [JsonIgnore]
    //[JsonProperty]
    public long DictId { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int DictSort { get; set; }
}
