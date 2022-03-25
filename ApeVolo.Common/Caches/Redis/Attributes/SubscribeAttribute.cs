using ApeVolo.Common.Caches.Redis.Abstractions;

namespace ApeVolo.Common.Caches.Redis.Attributes;

public class SubscribeAttribute : TopicAttribute
{
    public SubscribeAttribute(string name) : base(name)
    {
    }
}