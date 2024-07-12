using Ape.Volo.Common.Attributes;

namespace Ape.Volo.Common.ConfigOptions;

[OptionsSettings]
public class AopOptions
{
    public Tran Tran { get; set; }
    public Cache Cache { get; set; }
}

public class Tran
{
    public bool Enabled { get; set; }
}

public class Cache
{
    public bool Enabled { get; set; }
}
