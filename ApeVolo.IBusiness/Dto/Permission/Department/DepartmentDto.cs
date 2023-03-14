using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Permission.Department;

[AutoMapping(typeof(Entity.Do.Core.Department), typeof(DepartmentDto))]
public class DepartmentDto : EntityDtoRoot<long>
{
    [Display(Name = "Dept.Name")]
    public string Name { get; set; }

    [Display(Name = "Dept.PId")]
    public long? PId { get; set; }

    [Display(Name = "Dept.Sort")]
    public int Sort { get; set; }

    [Display(Name = "Dept.Enabled")]
    public bool Enabled { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<DepartmentDto> Children { get; set; }

    [Display(Name = "Dept.SubCount")]
    public int SubCount { get; set; }

    public bool HasChildren => SubCount > 0;

    public bool Leaf => SubCount == 0;

    public string Label
    {
        get { return Name; }
    }
}