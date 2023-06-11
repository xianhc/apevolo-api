using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class DepartmentExport : ExportBase
{
    [Display(Name = "Dept.Name")]
    public string Name { get; set; }

    [Display(Name = "Dept.PId")]
    public long ParentId { get; set; }

    [Display(Name = "Dept.Sort")]
    public int Sort { get; set; }

    [Display(Name = "Dept.Enabled")]
    public EnabledState EnabledState { get; set; }

    [Display(Name = "Dept.SubCount")]
    public int SubCount { get; set; }
}