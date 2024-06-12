using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.Common.Enums;

public enum TriggerType
{
    /// <summary>
    /// 表达式
    /// </summary>
    [Display(Name = "Cron")]
    Cron = 1,

    /// <summary>
    /// 简单的
    /// </summary>
    [Display(Name = "Simple")]
    Simple = 0
}
