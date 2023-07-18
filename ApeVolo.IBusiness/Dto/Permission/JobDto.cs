using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Job), typeof(JobDto))]
public class JobDto : BaseEntityDto<long>
{
    [Display(Name = "Job.Name")]
    public string Name { get; set; }

    [Display(Name = "Job.Sort")]
    public int Sort { get; set; }

    [Display(Name = "Job.Enabled")]
    public bool Enabled { get; set; }
}
