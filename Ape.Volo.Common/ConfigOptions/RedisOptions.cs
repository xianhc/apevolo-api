using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class RedisOptions
{
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
    public int Index { get; set; }
    public int ConnectTimeout { get; set; }
    public int SyncTimeout { get; set; }
    public int KeepAlive { get; set; }
    public int ConnectRetry { get; set; }
    public bool AbortOnConnectFail { get; set; }
    public bool AllowAdmin { get; set; }
    public int SuspendTime { get; set; }
    public int IntervalTime { get; set; }

    public int MaxQueueConsumption { get; set; }
    public bool ShowLog { get; set; }
}
