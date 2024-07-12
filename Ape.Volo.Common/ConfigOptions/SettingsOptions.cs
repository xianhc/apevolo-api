using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class SettingsOptions
{
    public bool IsInitTable { get; set; }
    public bool IsInitData { get; set; }
    public bool IsCqrs { get; set; }
    public bool IsQuickDebug { get; set; }
    public int FileLimitSize { get; set; }
    public string HmacSecret { get; set; }
    public string DefaultDataBase { get; set; }
    public string LogDataBase { get; set; }
}
