using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.Common.Enums;

public enum DictType
{
    /// <summary>
    /// 系统类
    /// </summary>
    [Display(Name = "系统类")]
    System = 1,

    /// <summary>
    /// 业务类
    /// </summary>
    [Display(Name = "业务类")]
    Business = 2
}
