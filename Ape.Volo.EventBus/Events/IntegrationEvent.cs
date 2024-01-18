using Newtonsoft.Json;

namespace Ape.Volo.EventBus.Events;

/// <summary>
/// 事件模型记录
/// </summary>
public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.Now;
    }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createDate)
    {
        Id = id;
        CreationDate = createDate;
    }

    [JsonProperty]
    public Guid Id { get; private init; }

    [JsonProperty]
    public DateTime CreationDate { get; private init; }
}
