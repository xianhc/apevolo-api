namespace Ape.Volo.Common.ConfigOptions;

public class CacheOption
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
