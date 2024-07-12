using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class EventBusOptions
{
    public bool Enabled { get; set; }
    public string SubscriptionClientName { get; set; }
}
