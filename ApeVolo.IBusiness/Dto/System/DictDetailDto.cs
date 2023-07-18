using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.System;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.System;

[AutoMapping(typeof(DictDetail), typeof(DictDetailDto))]
public class DictDetailDto : BaseEntityDto<long>
{
    [JsonIgnore]
    //[JsonProperty]
    public long DictId { get; set; }

    public string Label { get; set; }

    public string Value { get; set; }

    public string DictSort { get; set; }

    public DictDto2 Dict { get; set; }
}
