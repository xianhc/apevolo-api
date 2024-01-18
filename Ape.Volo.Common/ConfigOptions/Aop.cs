namespace Ape.Volo.Common.ConfigOptions;

public class Aop
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
