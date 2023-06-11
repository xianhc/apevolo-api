using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class RoleExport : ExportBase
{
    [Display(Name = "Role.Name")]
    public string Name { get; set; }

    [Display(Name = "Role.Level")]
    public int Level { get; set; }

    [Display(Name = "Role.Description")]
    public string Description { get; set; }

    [Display(Name = "Role.DataScope")]
    public string DataScope { get; set; }

    [Display(Name = "Role.DataDept")]
    public string DataDept { get; set; }

    [Display(Name = "Role.Permission")]
    public string Permission { get; set; }
}