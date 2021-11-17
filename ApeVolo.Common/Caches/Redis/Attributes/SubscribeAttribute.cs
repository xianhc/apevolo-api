using TopicAttribute = ApeVolo.Common.Caches.Redis.Abstractions.TopicAttribute;

namespace ApeVolo.Common.Caches.Redis.Attributes
{
    public class SubscribeAttribute : TopicAttribute
    {
        public SubscribeAttribute(string name) : base(name)
        {
        }
    }
}