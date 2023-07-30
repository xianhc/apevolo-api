using System;

namespace ApeVolo.Common.Caches;

public class ValueInfoEntry
{
    public string Value { get; set; }
    public string TypeName { get; set; }
    public TimeSpan ExpireTime { get; set; }
    public CacheExpireType ExpireType { get; set; }
}
