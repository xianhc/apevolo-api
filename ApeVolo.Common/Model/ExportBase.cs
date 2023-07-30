using System;
using System.ComponentModel.DataAnnotations;

namespace ApeVolo.Common.Model;

public class ExportBase
{
    [Display(Name = "创建时间")]
    public DateTime CreateTime { get; set; }

    [Display(Name = "ID")]
    public long Id { get; set; }
}
