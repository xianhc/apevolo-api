using System;
using System.ComponentModel.DataAnnotations;

namespace ApeVolo.Common.Model;

public class ExportBase
{
    [Display(Name = "Sys.CreateTime")]
    public DateTime CreateTime { get; set; }

    [Display(Name = "Sys.Id")]
    public long Id { get; set; }
}