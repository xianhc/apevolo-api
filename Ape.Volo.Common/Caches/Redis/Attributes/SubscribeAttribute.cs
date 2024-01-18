using Ape.Volo.Common.Caches.Redis.Abstractions;

namespace Ape.Volo.Common.Caches.Redis.Attributes;

public class SubscribeAttribute : TopicAttribute
{
    public SubscribeAttribute(string name) : base(name)
    {
    }
}
