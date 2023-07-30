using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class JobExport : ExportBase
{
    [Display(Name = "岗位名称")]
    public string Name { get; set; }

    [Display(Name = "排序")]
    public int Sort { get; set; }

    [Display(Name = "是否启用")]
    public EnabledState EnabledState { get; set; }
}
