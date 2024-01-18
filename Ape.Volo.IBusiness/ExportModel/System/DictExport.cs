using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

public class DictExport : ExportBase
{
    [Display(Name = "字典名称")]
    public string Name { get; set; }

    [Display(Name = "字典描述")]
    public string Description { get; set; }

    [Display(Name = "字典标签")]
    public string Lable { get; set; }

    [Display(Name = "字典值")]
    public string Value { get; set; }
}
