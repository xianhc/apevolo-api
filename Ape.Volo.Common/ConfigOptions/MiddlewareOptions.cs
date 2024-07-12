using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class MiddlewareOptions
{
    public QuartzNetJob QuartzNetJob { get; set; }
    public IpLimit IpLimit { get; set; }
    public MiniProfiler MiniProfiler { get; set; }
    public RabbitMq RabbitMq { get; set; }
    public RedisMq RedisMq { get; set; }
    public Elasticsearch Elasticsearch { get; set; }
}

public class QuartzNetJob
{
    public bool Enabled { get; set; }
}

public class IpLimit
{
    public bool Enabled { get; set; }
}

public class MiniProfiler
{
    public bool Enabled { get; set; }
}

public class RabbitMq
{
    public bool Enabled { get; set; }
}

public class RedisMq
{
    public bool Enabled { get; set; }
}

public class Elasticsearch
{
    public bool Enabled { get; set; }
}
