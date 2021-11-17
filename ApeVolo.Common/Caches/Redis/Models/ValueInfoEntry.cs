using ApeVolo.Common.Global;
using System;

namespace ApeVolo.Common.Caches.Redis.Models
{
    class ValueInfoEntry
    {
        public string Value { get; set; }
        public string TypeName { get; set; }
        public TimeSpan? ExpireTime { get; set; }
        public RedisExpireType? ExpireType { get; set; }
    }
}
