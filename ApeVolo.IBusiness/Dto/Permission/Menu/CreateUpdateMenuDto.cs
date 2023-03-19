using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Menu;

[AutoMapping(typeof(Entity.Permission.Menu), typeof(CreateUpdateMenuDto))]
public class CreateUpdateMenuDto : EntityDtoRoot<long>
{
    [Display(Name = "Mneu.Title")]
    [Required(ErrorMessage = "{0}required")]
    public string Title { get; set; }

    public string LinkUrl { get; set; }

    public string Path { get; set; }

    public string Permission { get; set; }

    public bool IFrame { get; set; }

    public string Component { get; set; }

    public string ComponentName { get; set; }

    public long? PId { get; set; }

    [Display(Name = "Menu.Sort")]
    [Range(1, 999, ErrorMessage = "{0}range{1}{2}")]
    public int MenuSort { get; set; }

    public string Icon { get; set; }


    [Display(Name = "Menu.Type")]
    [Range(1, 3, ErrorMessage = "{0}range{1}{2}")]
    public int Type { get; set; }

    public bool Cache { get; set; }

    public bool Hidden { get; set; }

    public int SubCount { get; set; }
}