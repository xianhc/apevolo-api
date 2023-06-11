using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Global;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class SettingExport : ExportBase
{
    [Display(Name = "Setting.Name")]
    public string Name { get; set; }

    [Display(Name = "Setting.Value")]
    public string Value { get; set; }

    [Display(Name = "Setting.Enabled")]
    public EnabledState EnabledState { get; set; }

    [Display(Name = "Setting.Description")]
    public string Description { get; set; }
}