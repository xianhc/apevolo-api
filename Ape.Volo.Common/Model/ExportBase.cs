using System;
using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.Common.Model;

public class ExportBase
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [Display(Name = "创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// ID
    /// </summary>
    [Display(Name = "ID")]
    public long Id { get; set; }
}
