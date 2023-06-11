using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.Permission;

public class JobExport : ExportBase
{
    [Display(Name = "Job.Name")]
    public string Name { get; set; }

    [Display(Name = "Job.Sort")]
    public int Sort { get; set; }

    [Display(Name = "Job.Enabled")]
    public EnabledState EnabledState { get; set; }
}