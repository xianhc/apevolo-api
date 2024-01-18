using System;

namespace Ape.Volo.Common.Caches;

public class ValueInfoEntry
{
    public string Value { get; set; }
    public string TypeName { get; set; }
    public TimeSpan ExpireTime { get; set; }
    public CacheExpireType ExpireType { get; set; }
}
