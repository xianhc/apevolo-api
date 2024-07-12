using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class CacheOptions
{
    public RedisCacheSwitch RedisCacheSwitch { get; set; }
    public DistributedCacheSwitch DistributedCacheSwitch { get; set; }
}

public class RedisCacheSwitch
{
    public bool Enabled { get; set; }
}

public class DistributedCacheSwitch
{
    public bool Enabled { get; set; }
}
