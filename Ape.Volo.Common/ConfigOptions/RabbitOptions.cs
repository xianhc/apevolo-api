using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class RabbitOptions
{
    public bool Enabled { get; set; }
    public string Connection { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RetryCount { get; set; }
}
