using Ape.Volo.Common.Caches.Redis.Abstractions;

namespace Ape.Volo.Common.Caches.Redis.Attributes;

public class SubscribeDelayAttribute : TopicAttribute
{
    public SubscribeDelayAttribute(string name) : base(name)
    {
    }
}
