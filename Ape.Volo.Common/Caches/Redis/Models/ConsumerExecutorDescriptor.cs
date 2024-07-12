using System.Reflection;
using Ape.Volo.Common.Attributes.Redis;

namespace Ape.Volo.Common.Caches.Redis.Models;

public class ConsumerExecutorDescriptor
{
    public MethodInfo MethodInfo { get; set; }

    public TypeInfo ImplTypeInfo { get; set; }

    public TopicAttribute Attribute { get; set; }
}
