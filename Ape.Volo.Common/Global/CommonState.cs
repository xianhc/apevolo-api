using System.ComponentModel.DataAnnotations;

namespace Ape.Volo.Common.Global;

public enum CommonState
{
}

public enum EnabledState
{
    [Display(Name = "启用")]
    Enabled = 1,

    [Display(Name = "禁用")]
    Disabled = 0
}

public enum BoolState
{
    [Display(Name = "是")]
    True = 1,

    [Display(Name = "否")]
    False = 0
}
