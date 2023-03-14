using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Job;

[AutoMapping(typeof(Entity.Do.Core.Job), typeof(CreateUpdateJobDto))]
public class CreateUpdateJobDto : EntityDtoRoot<long>
{
    [Display(Name = "Job.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    [Display(Name = "Job.Sort")]
    [Range(1, 999, ErrorMessage = "{0}range{1}{2}")]
    public int Sort { get; set; }

    public bool Enabled { get; set; }
}