using System.Reflection;
using ApeVolo.Common.Caches.Redis.Abstractions;

namespace ApeVolo.Common.Caches.Redis.Models;

public class ConsumerExecutorDescriptor
{
    public MethodInfo MethodInfo { get; set; }

    public TypeInfo ImplTypeInfo { get; set; }

    public TopicAttribute Attribute { get; set; }
}