using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Department), typeof(DepartmentDto))]
public class DepartmentDto : BaseEntityDto<long>
{
    public string Name { get; set; }

    public long? ParentId { get; set; }

    public int Sort { get; set; }

    public bool Enabled { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<DepartmentDto> Children { get; set; }

    public int SubCount { get; set; }

    public bool HasChildren => SubCount > 0;

    public bool Leaf => SubCount == 0;

    public string Label
    {
        get { return Name; }
    }
}
