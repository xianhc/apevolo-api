namespace Ape.Volo.Common.Attributes.Redis;

public class SubscribeDelayAttribute : TopicAttribute
{
    public SubscribeDelayAttribute(string name, bool bulk = false) : base(name, bulk)
    {
    }
}
