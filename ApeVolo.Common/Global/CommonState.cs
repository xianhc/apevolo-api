using System.ComponentModel.DataAnnotations;

namespace ApeVolo.Common.Global;

public enum CommonState
{
}

public enum EnabledState
{
    [Display(Name = "Sys.Enabled")] Enabled = 1,
    [Display(Name = "Sys.Disabled")] Disabled = 0
}

public enum BoolState
{
    [Display(Name = "Sys.True")] True = 1,
    [Display(Name = "Sys.False")] False = 0
}