using System;

namespace Ape.Volo.Common.Attributes.Redis;

/// <inheritdoc />
/// <summary>
/// An abstract attribute that for kafka attribute or rabbit mq attribute
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public abstract class TopicAttribute : Attribute
{
    protected TopicAttribute(string name, bool bulk = false)
    {
        Name = name;
        Bulk = bulk;
    }

    /// <summary>
    /// Topic or exchange route key name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Default group name is CapOptions setting.(Assembly name)
    /// kafka --> groups.id
    /// rabbit MQ --> queue.name
    /// </summary>
    public string Group { get; set; }


    /// <summary>
    /// 批量消费 参数是必须的
    /// </summary>
    public bool Bulk { get; set; }
}
