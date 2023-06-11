using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.Model;

namespace ApeVolo.IBusiness.ExportModel.System;

public class DictExport : ExportBase
{
    [Display(Name = "Dict.Name")]
    public string Name { get; set; }

    [Display(Name = "Dict.Description")]
    public string Description { get; set; }

    [Display(Name = "Dict.Detail.Lable")]
    public string Lable { get; set; }

    [Display(Name = "Dict.Detail.Value")]
    public string Value { get; set; }
}