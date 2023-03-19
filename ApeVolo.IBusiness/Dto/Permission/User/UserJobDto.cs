using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.User;

[AutoMapping(typeof(Entity.Permission.Job), typeof(UserJobDto))]
public class UserJobDto : RootId<long>
{
    [Display(Name = "Job.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }
}